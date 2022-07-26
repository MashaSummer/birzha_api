using Facade.Web.Application;
using Grpc.Core;
using Grpc.Net.Client;
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
    public override async Task<GetAllProductsResponse> GetAllProducts(
        GetAllProductsRequest request, 
        ServerCallContext context)
    {
        var userId = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");

        _logger.LogInformation($"{nameof(GetAllProducts)} request by user: id={userId}");

        var channel = GrpcChannel.ForAddress(_serviceUrls.ProductService);
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(channel);

        var response = await productClient.GetAllProductsAsync(new GetAllProductsRequest());
        if (response == null)
        {
            var error = new Error() { ErrorMessage = "Failed to request" };
            response = new GetAllProductsResponse() { Error = error };
            _logger.LogError($"{nameof(GetAllProducts)} failed to request", new Exception());
        }
        else if (response.Error != null)
        {
            _logger.LogError($"{nameof(GetAllProducts)} something went wrong in ProductMicroservice", response.Error);
        }

        return await Task.FromResult(response); 
    }
}

