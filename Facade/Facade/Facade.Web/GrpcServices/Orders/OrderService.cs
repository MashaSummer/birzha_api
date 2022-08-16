using Facade.Web.Application;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orders;

namespace Facade.Web.GrpcServices.Orders;

[Authorize]
public class OrderService : OrdersService.OrdersServiceBase
{
    private readonly OrdersService.OrdersServiceClient _client;
    
    
    public OrderService(IOptionsMonitor<ServiceUrls> optionsMonitor)
    {
        var channel = GrpcChannel.ForAddress(optionsMonitor.CurrentValue.OrdersService);
        _client = new OrdersService.OrdersServiceClient(channel);
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context) => 
        await _client.CreateOrderAsync(request);

    public override async Task<ProductInfoResponse> GetProductsInfo(ProductInfoRequest request, ServerCallContext context) => 
        await _client.GetProductsInfoAsync(request);

    public override async Task<UserProductsResponse> GetUserProductsInfo(UserProductsRequest request, ServerCallContext context) => 
        await _client.GetUserProductsInfoAsync(request);
}