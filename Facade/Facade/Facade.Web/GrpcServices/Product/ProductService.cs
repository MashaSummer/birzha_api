using Calabonga.OperationResults;
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
        var channel = GrpcChannel.ForAddress(_serviceUrls.ProductService);
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(channel);

        var response = await TryGetAllProducts(productClient);

        if (response.Ok)
        {
            return response.Result;
        }

        return new GetAllProductsResponse()
        {
            Error = new Error()
            {
                ErrorMessage = response.Exception == null ? "Failed to request" : response.Exception.Message,
                StackTrace = response.Exception == null ? new Exception().StackTrace : response.Exception.StackTrace
            }
        };
    }


    private async Task<OperationResult<GetAllProductsResponse>> TryGetAllProducts(ProductGrpc.ProductService.ProductServiceClient client)
    {
        var result = OperationResult.CreateResult<GetAllProductsResponse>();

        try
        {
            result.Result = await client.GetAllProductsAsync(new GetAllProductsRequest());
            if (result.Result == null)
            {
                result.AddError(new Exception("Failed to request"));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }
}

