using Facade.Web.Application;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ProductGrpc;

namespace Facade.Web.GrpcServices.Product;

[Authorize]
public class ProductService : ProductGrpc.ProductService.ProductServiceBase
{
    private readonly ILogger<ProductService> _logger;
    private readonly ServiceUrls _serviceUrls;
    public ProductService(
        ILogger<ProductService> logger,
        IOptionsMonitor<ServiceUrls> optionsMonitor
        )
    {
        _logger = logger;
        _serviceUrls = optionsMonitor.CurrentValue;
    }
    public override Task<GetAllProductsResponse> GetAllProducts(
        GetAllProductsRequest request, 
        ServerCallContext context)
    {
        var userId = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
        _logger.LogInformation($"GetAllProducts request by user: id={userId}");




    }
}

