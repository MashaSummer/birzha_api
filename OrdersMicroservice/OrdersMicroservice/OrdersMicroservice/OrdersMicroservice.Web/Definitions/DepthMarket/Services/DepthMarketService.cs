using AutoMapper;
using Confluent.Kafka;
using OrdersEvent;
using OrdersMicroservice.Definitions.DepthMarket.Repository;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.EventsBase;

namespace OrdersMicroservice.Definitions.DepthMarket.Services;

public class DepthMarketService
{
    private readonly AskMarketRepository _askMarketRepository;
    private readonly IMapper _mapper;
    private readonly OrdersRepository _ordersRepository;
    private readonly BidMarketRepository _bidMarketRepository;
    private readonly DepthMarketSearchService _depthMarketSearchService;
    private readonly IEventProducer<Null, CandidatesFoundEvent> _eventProducer;

    public DepthMarketService(
        AskMarketRepository askMarketRepository,
        BidMarketRepository bidMarketRepository,
        OrdersRepository ordersRepository,
        IMapper mapper,
        DepthMarketSearchService depthMarketSearchService, IEventProducer<Null, CandidatesFoundEvent> eventProducer)
    {
        _askMarketRepository = askMarketRepository;
        _ordersRepository = ordersRepository;
        _bidMarketRepository = bidMarketRepository;
        _mapper = mapper;
        _depthMarketSearchService = depthMarketSearchService;
        _eventProducer = eventProducer;
    }

    public async Task ProcessOrderAsync(string orderId)
    {
        
        var order = await _ordersRepository.GetByIdAsync(orderId);

        if (order.OrderType is OrderTypes.Ask)
        {
            await HandleAskAsync(order);
        }
        else
        {
            await HandleBidAsync(order);
        }
    }
    private async Task HandleAskAsync(OrderModel order)
    {
        var relevantBids = await _bidMarketRepository.GetRelevantBidsAsync(order);
        if (order.OnlyFullExecution)
        {
            var matchedMarketModel = _depthMarketSearchService.FullExecSearch(order, relevantBids);
            if (matchedMarketModel == null)
            {
                order.Status = OrderStatus.Active;
                await _ordersRepository.UpdateAsync(order);
                var marketModel = _mapper.Map<MarketModel>(order);
                await _askMarketRepository.CreateNewAsync(marketModel);
            }
            else
            {
                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);
                if (matchedMarketModel.OnlyFullExecution)
                {
                    matchedOrder.Status = OrderStatus.Executing;
                    order.Status = OrderStatus.Executing;
                    
                    await _ordersRepository.UpdateAsync(matchedOrder);
                    await _ordersRepository.UpdateAsync(order);
                    await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
                    
                    await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                    {
                        AskCandidate = new Candidate()
                        {
                            OrderId = order.Id,
                            Volume = order.Volume
                        },

                        BidCandidate = new Candidate()
                        {
                            OrderId = matchedOrder.Id,
                            Volume = matchedOrder.Volume
                        }
                    });

                }
                else
                {
                    var newVolume = matchedMarketModel.Volume - order.Volume;
                    if (newVolume != 0)
                    {

                        matchedMarketModel.Volume = newVolume;
                        await _bidMarketRepository.UpdateAsync(matchedMarketModel);

                        order.Status = OrderStatus.Executing;
                        await _ordersRepository.UpdateAsync(order);


                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            AskCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            BidCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedOrder.Volume
                            }
                        });
                    }
                    else
                    {
                        matchedOrder.Status = OrderStatus.Executing;
                        order.Status = OrderStatus.Executing;
                        
                        await _ordersRepository.UpdateAsync(matchedOrder);
                        await _ordersRepository.UpdateAsync(order);
                        await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            AskCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            BidCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedOrder.Volume
                            }
                        });

                    }
                }
            }
        }
        else
        {
            var matchedMarketModels = _depthMarketSearchService.PartialExecSearch(order, relevantBids);
            if (!matchedMarketModels.Any())
            {
                //in separate method put logic of change status and add to market repo
                order.Status = OrderStatus.Active;
                await _ordersRepository.UpdateAsync(order);

                var marketModel = _mapper.Map<MarketModel>(order);
                await _askMarketRepository.CreateNewAsync(marketModel);
            }
            else
            {
                var orderVolume = order.Volume;
                foreach (var matchedMarketModel in matchedMarketModels)
                {
                    if (!matchedMarketModel.OnlyFullExecution &&
                        matchedMarketModel.Volume > orderVolume)
                    {
                        order.Status = OrderStatus.Executing;
                        await _ordersRepository.UpdateAsync(order);

                        matchedMarketModel.Volume -= orderVolume;
                        orderVolume = 0;
                        await _bidMarketRepository.UpdateAsync(matchedMarketModel);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            AskCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            BidCandidate = new Candidate()
                            {
                                OrderId = matchedMarketModel.OrderId,
                                Volume = matchedMarketModel.Volume + order.Volume
                            }
                        });
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);

                        matchedOrder.Status = OrderStatus.Executing;
                        await _ordersRepository.UpdateAsync(matchedOrder);
                        orderVolume -= matchedMarketModel.Volume;
                        await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);

                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            AskCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = orderVolume + matchedMarketModel.Volume
                            },

                            BidCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedMarketModel.Volume
                            }
                        });
                    }

                }
                if (orderVolume == 0)
                {
                    order.Status = OrderStatus.Executing;
                    await _ordersRepository.UpdateAsync(order);
                }
                else
                {
                    order.Status = OrderStatus.Active;
                    await _ordersRepository.UpdateAsync(order);
                    var marketModel = _mapper.Map<MarketModel>(order);
                    marketModel.Volume = orderVolume;
                    await _askMarketRepository.CreateNewAsync(marketModel);
                }
            }
        }
    }
    private async Task HandleBidAsync(OrderModel order)
    {
        var relevantAsks = await _askMarketRepository.GetRelevantAsksAsync(order);
        if (order.OnlyFullExecution)
        {
            var matchedMarketModel = _depthMarketSearchService.FullExecSearch(order, relevantAsks);
            if (matchedMarketModel == null)
            {
                order.Status = OrderStatus.Active;
                await _ordersRepository.UpdateAsync(order);

                var marketModel = _mapper.Map<MarketModel>(order);
                await _bidMarketRepository.CreateNewAsync(marketModel);
            }
            else
            {
                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);
                if (matchedMarketModel.OnlyFullExecution)
                {
                    matchedOrder.Status = OrderStatus.Executing;
                    order.Status = OrderStatus.Executing;
                    
                    await _ordersRepository.UpdateAsync(matchedOrder);
                    await _ordersRepository.UpdateAsync(order);
                    await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
                    
                    await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                    {
                        BidCandidate = new Candidate()
                        {
                            OrderId = order.Id,
                            Volume = order.Volume
                        },

                        AskCandidate = new Candidate()
                        {
                            OrderId = matchedOrder.Id,
                            Volume = matchedOrder.Volume
                        }
                    });
                }
                else
                {
                    var newVolume = matchedMarketModel.Volume - order.Volume;
                    if (newVolume != 0)
                    {

                        matchedMarketModel.Volume = newVolume;
                        await _askMarketRepository.UpdateAsync(matchedMarketModel);

                        order.Status = OrderStatus.Executing;
                        await _ordersRepository.UpdateAsync(order);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            BidCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            AskCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedOrder.Volume
                            }
                        });
                    }
                    else
                    {
                        matchedOrder.Status = OrderStatus.Executing;
                        order.Status = OrderStatus.Executing;
                        
                        await _ordersRepository.UpdateAsync(matchedOrder);
                        await _ordersRepository.UpdateAsync(order);
                        await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            BidCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            AskCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedOrder.Volume
                            }
                        });

                    }
                }
            }
        }
        else
        {
            var matchedMarketModels = _depthMarketSearchService.PartialExecSearch(order, relevantAsks);
            if (!matchedMarketModels.Any())
            {
                //in separate method put logic of change status and add to market repo
                order.Status = OrderStatus.Active;
                await _ordersRepository.UpdateAsync(order);

                var marketModel = _mapper.Map<MarketModel>(order);
                await _bidMarketRepository.CreateNewAsync(marketModel);
            }
            else
            {
                var orderVolume = order.Volume;
                foreach (var matchedMarketModel in matchedMarketModels)
                {
                    if (!matchedMarketModel.OnlyFullExecution &&
                        matchedMarketModel.Volume > orderVolume)
                    {
                        order.Status = OrderStatus.Executing; // status problem in partial large orders
                        await _ordersRepository.UpdateAsync(order);

                        matchedMarketModel.Volume -= orderVolume;
                        orderVolume = 0;
                        await _askMarketRepository.UpdateAsync(matchedMarketModel);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            BidCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = order.Volume
                            },

                            AskCandidate = new Candidate()
                            {
                                OrderId = matchedMarketModel.OrderId,
                                Volume = matchedMarketModel.Volume + order.Volume
                            }
                        });
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);

                        matchedOrder.Status = OrderStatus.Executing;
                        await _ordersRepository.UpdateAsync(matchedOrder);
                        orderVolume -= matchedMarketModel.Volume;
                        await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
                        
                        await _eventProducer.ProduceAsync(null, new CandidatesFoundEvent()
                        {
                            BidCandidate = new Candidate()
                            {
                                OrderId = order.Id,
                                Volume = orderVolume + matchedMarketModel.Volume
                            },

                            AskCandidate = new Candidate()
                            {
                                OrderId = matchedOrder.Id,
                                Volume = matchedMarketModel.Volume
                            }
                        });
                    }

                }
                if (orderVolume == 0)
                {
                    order.Status = OrderStatus.Executing;
                    await _ordersRepository.UpdateAsync(order);
                }
                else
                {
                    order.Status = OrderStatus.Active;
                    await _ordersRepository.UpdateAsync(order);
                    var marketModel = _mapper.Map<MarketModel>(order);
                    marketModel.Volume = orderVolume;
                    await _bidMarketRepository.CreateNewAsync(marketModel);
                }
            }
        }
    }
    
}

