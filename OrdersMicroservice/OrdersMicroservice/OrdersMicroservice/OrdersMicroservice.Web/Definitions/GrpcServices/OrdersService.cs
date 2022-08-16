using AutoMapper;
using Confluent.Kafka;
using Google.Protobuf.Collections;
using Grpc.Core;
using Orders;
using OrdersEvent;
using OrdersMicroservice.Definitions.DepthMarket.Services;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Domain.EventsBase;

namespace OrdersMicroservice.Definitions.GrpcServices;

public class OrdersService : Orders.OrdersService.OrdersServiceBase
{
    private readonly IRepository<OrderModel> _repository;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, OrderCreatedEvent> _eventProducer;
    private readonly DepthMarketSearchService _depthMarketSearchService;

    public OrdersService(
        IRepository<OrderModel> repository, 
        IMapper mapper, 
        IEventProducer<Null, OrderCreatedEvent> eventProducer,
        DepthMarketSearchService depthMarketSearchService)
    {
        _repository = repository;
        _mapper = mapper;
        _eventProducer = eventProducer;
        _depthMarketSearchService = depthMarketSearchService;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        var order = _mapper.Map<OrderModel>(request.OrderDetail);
        var hasAsk = await _depthMarketSearchService.HasAskAsync(order.ProductId);
        
        order.Status = hasAsk || order.OrderType == OrderTypes.Ask ? OrderStatus.Validating : OrderStatus.Aborted;
        var id = await _repository.AddAsync(order);

        if (!hasAsk && order.OrderType == OrderTypes.Bid)
        {
            return new CreateOrderResponse()
            {
                Error = new Orders.Error()
                {
                    ErrorText = "Order aborted"
                }
            };
        }

        await _eventProducer.ProduceAsync(null, new OrderCreatedEvent()
        {
            Order = request.OrderDetail,
            OrderId = id.Result
        });

        return new CreateOrderResponse()
        {
            Success = new Success()
            {
                SuccessText = "Order is processing"
            }
        };
    }

    public override async Task<UserProductsResponse> GetUserProductsInfo(UserProductsRequest request,
        ServerCallContext context)
    {
        try
        {
            var productInfoDtos =
                await _depthMarketSearchService.GetProductsInfoAsync(request.InvestorId, request.ProductsId.ToList());

            var response = new UserProductsResponse()
            {
                Success = new UserProductInfoArray()
            };

            response.Success.Products.AddRange(productInfoDtos.Select(p => _mapper.Map<UserProductInfo>(p)));

            return response;
        }
        catch (Exception e)
        {
            return new UserProductsResponse()
            {
                Error = new Orders.Error()
                {
                    ErrorText = e.Message,
                    StackTrace = e.StackTrace
                }
            };
        }
    }

    public override async Task<ProductInfoResponse> GetProductsInfo(ProductInfoRequest request, ServerCallContext context)
    {
        try
        {
            var infoList = new List<ProductInfo>();
            foreach (var productId in request.ProductsId)
            {
                infoList.Add(new ProductInfo()
                {
                    BestAsk = await _depthMarketSearchService.GetBestAskAsync(productId),
                    BestBid = await _depthMarketSearchService.GetBestBidAsync(productId),
                    ProductId = productId
                });
            }

            var response = new ProductInfoResponse();
            response.Success.ProductsInfo.Add(infoList);
            return response;
        }
        catch (Exception e)
        {
            return new ProductInfoResponse()
            {
                Error = new Orders.Error()
                {
                    ErrorText = e.Message,
                    StackTrace = e.StackTrace
                }
            };
        }
    }
}