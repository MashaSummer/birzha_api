namespace OrdersMicroservice.Definitions.DepthMarket
{
    public class DepthMarketService : IDepthMarketService
    {
        private readonly IAskMarketRepository _askMarketRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IBidMarketRepository _bidMarketRepository;
        public DepthMarketService(
            IAskMarketRepository askMarketRepository,
            IOrdersRepository ordersRepository,
            IBidMarketRepository bidMarketRepository
            )
        {
            _askMarketRepository = askMarketRepository;
            _ordersRepository = ordersRepository;
            _bidMarketRepository = bidMarketRepository;
        }
        public async Task ProcessOrderAsync(string orderId)
        {
            var order = await _ordersRepository.GetByIdAsync(orderId);

            if (order.OrderType is OrderTypes.Ask)
            {
                var relevantBids = await _bidMarketRepository.GetRelevantBidsAsync(order);
                if (order.OnlyFullExecution)
                {
                    var matchedMarketModel = FullExecSearch(order, relevantBids);
                    if (matchedMarketModel == null)
                    {
                        order.Status = OrderStatus.Active;
                        await _ordersRepository.UpdateAsync(order);

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _askMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);
                        if (matchedMarketModel.OnlyFullExecution)
                        {
                            matchedOrder.Status = OrderStatus.Executing;
                            order.Status = OrderStatus.Executing;
                            // send in candidates also send a volume just like an idea

                            await _ordersRepository.UpdateAsync(matchedOrder);
                            await _ordersRepository.UpdateAsync(order);
                            await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
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
                                // send in candidates
                            }
                            else
                            {
                                matchedOrder.Status = OrderStatus.Executing;
                                order.Status = OrderStatus.Executing;
                                // send in candidates

                                await _ordersRepository.UpdateAsync(matchedOrder);
                                await _ordersRepository.UpdateAsync(order);
                                await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
                            }
                        }
                    }
                }
                else
                {
                    var matchedMarketModels = PartialExecSearch(order, relevantBids);
                    if (!matchedMarketModels.Any())
                    {
                        //in separet method put logic of change status and add to market repo
                        order.Status = OrderStatus.Active;
                        await _ordersRepository.UpdateAsync(order);

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _askMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        double orderVolume = order.Volume;
                        foreach (var matchedMarketModel in matchedMarketModels)
                        {
                            if (!matchedMarketModel.OnlyFullExecution &&
                                matchedMarketModel.Volume > order.Volume)
                            {
                                order.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(order);

                                matchedMarketModel.Volume -= orderVolume;
                                orderVolume = 0;
                                await _bidMarketRepository.UpdateAsync(matchedMarketModel);
                                // send in candidates
                            }
                            else
                            {
                                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);

                                matchedOrder.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(matchedOrder);
                                orderVolume -= matchedMarketModel.Volume;
                                await _bidMarketRepository.DeleteAsync(matchedMarketModel.Id);
                                // send in candidates
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
                            var marketModel = new MarketModel()
                            {
                                ProductId = order.ProductId,
                                OrderId = order.Id,
                                Volume = orderVolume,
                                Price = order.Price,
                                OnlyFullExecution = order.OnlyFullExecution,
                                SubmissionTime = order.SubmissionTime
                            };
                            await _askMarketRepository.CreateNewAsync(marketModel);
                        }
                    }
                }
            }
            else
            {
                var relevantAsks = await _askMarketRepository.GetRelevantAsksAsync(order);
                if (order.OnlyFullExecution)
                {
                    var matchedMarketModel = FullExecSearch(order, relevantAsks);
                    if (matchedMarketModel == null)
                    {
                        order.Status = OrderStatus.Active;
                        await _ordersRepository.UpdateAsync(order);

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _bidMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);
                        if (matchedMarketModel.OnlyFullExecution)
                        {
                            matchedOrder.Status = OrderStatus.Executing;
                            order.Status = OrderStatus.Executing;
                            // send in candidates also send a volume just like an idea

                            await _ordersRepository.UpdateAsync(matchedOrder);
                            await _ordersRepository.UpdateAsync(order);
                            await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
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
                                // send in candidates
                            }
                            else
                            {
                                matchedOrder.Status = OrderStatus.Executing;
                                order.Status = OrderStatus.Executing;
                                // send in candidates

                                await _ordersRepository.UpdateAsync(matchedOrder);
                                await _ordersRepository.UpdateAsync(order);
                                await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
                            }
                        }
                    }
                }
                else
                {
                    var matchedMarketModels = PartialExecSearch(order, relevantAsks);
                    if (!matchedMarketModels.Any())
                    {
                        //in separet method put logic of change status and add to market repo
                        order.Status = OrderStatus.Active;
                        await _ordersRepository.UpdateAsync(order);

                        var marketModel = new MarketModel()
                        {
                            ProductId = order.ProductId,
                            OrderId = order.Id,
                            Volume = order.Volume,
                            Price = order.Price,
                            OnlyFullExecution = order.OnlyFullExecution,
                            SubmissionTime = order.SubmissionTime
                        };
                        await _bidMarketRepository.CreateNewAsync(marketModel);
                    }
                    else
                    {
                        double orderVolume = order.Volume;
                        foreach (var matchedMarketModel in matchedMarketModels)
                        {
                            if (!matchedMarketModel.OnlyFullExecution &&
                                matchedMarketModel.Volume > order.Volume)
                            {
                                order.Status = OrderStatus.Executing; // status problem in partial large orders
                                await _ordersRepository.UpdateAsync(order);

                                matchedMarketModel.Volume -= orderVolume;
                                orderVolume = 0;
                                await _askMarketRepository.UpdateAsync(matchedMarketModel);
                                // send in candidates
                            }
                            else
                            {
                                var matchedOrder = await _ordersRepository.GetByIdAsync(matchedMarketModel.OrderId);

                                matchedOrder.Status = OrderStatus.Executing;
                                await _ordersRepository.UpdateAsync(matchedOrder);
                                orderVolume -= matchedMarketModel.Volume;
                                await _askMarketRepository.DeleteAsync(matchedMarketModel.Id);
                                // send in candidates
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
                            var marketModel = new MarketModel()
                            {
                                ProductId = order.ProductId,
                                OrderId = order.Id,
                                Volume = orderVolume,
                                Price = order.Price,
                                OnlyFullExecution = order.OnlyFullExecution,
                                SubmissionTime = order.SubmissionTime
                            };
                            await _bidMarketRepository.CreateNewAsync(marketModel);
                        }
                    }
                }
            }
        }
        private MarketModel FullExecSearch(OrderModel model, List<MarketModel> relevantMarketList)
        {
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if (model.Volume == listItem.Volume)
                    {
                        return listItem;
                    }
                    continue;
                }
                if (listItem.Volume >= model.Volume)
                    return listItem;
            }
            return null;
        }
        private List<MarketModel> PartialExecSearch(OrderModel model, List<MarketModel> relevantMarketList)
        {
            var existingVolume = model.Volume;
            var candidatesList = new List<MarketModel>();
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution) // refactoring by doing if like in processing pull
                {
                    if (existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                }
                else
                {
                    if (existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                    else
                    {
                        candidatesList.Add(listItem);
                        break;
                    }
                }
            }
            return candidatesList;
        }
    }
}
