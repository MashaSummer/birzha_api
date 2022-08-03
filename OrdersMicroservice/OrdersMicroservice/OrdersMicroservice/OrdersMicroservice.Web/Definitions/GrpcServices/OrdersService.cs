using AutoMapper;
using Grpc.Core;
using Orders;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Domain.DbBase;

namespace OrdersMicroservice.Definitions.GrpcServices;

public class OrdersService : Orders.OrdersService.OrdersServiceBase
{
    private readonly IDbWorker<OrderModel> _dbWorker;
    private readonly IMapper _mapper;

    public OrdersService(IDbWorker<OrderModel> dbWorker, IMapper mapper)
    {
        _dbWorker = dbWorker;
        _mapper = mapper;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        // TODO send request to validate
        
        // TODO move to kafka handler

        var order = _mapper.Map<OrderModel>(request);
        order.Status = OrderStatus.Validating;
        await _dbWorker.AddNewRecord(order);

        return new CreateOrderResponse()
        {
            Success = new Success()
            {
                SuccessText = "Order is processing"
            }
        };
    }
}