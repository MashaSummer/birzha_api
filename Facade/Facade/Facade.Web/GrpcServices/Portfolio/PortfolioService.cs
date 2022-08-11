using Calabonga.OperationResults;
using Facade.Web.Application;
using Facade.Web.GrpcServices.Product;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ProductGrpc;

namespace Facade.Web.GrpcServices.Portfolio;

[Authorize]
public class PortfolioService : PortfolioServiceGrpc.PortfolioService.PortfolioServiceBase
{
    private readonly ILogger<PortfolioService> _logger;
    private readonly ServiceUrls _serviceUrls;

    public PortfolioService(
        ILogger<PortfolioService> logger,
        IOptionsMonitor<ServiceUrls> optionsMonitor)
    { 
        _logger = logger;
        _serviceUrls = optionsMonitor.CurrentValue;
    }

    public override Task<PortfolioServiceGrpc.PortfolioResponse> GetPortfolio(PortfolioServiceGrpc.GetPortfolioRequest request, ServerCallContext context)
    {

    }

    public override async Task<GetAllAssetsResponse> GetAllAssets(GetAllAssetsRequest request, ServerCallContext context)
    {
        var channel = GrpcChannel.ForAddress(_serviceUrls.PortfolioService);
        var portfolioClient = new PortfolioGrpc.PortfolioService.PortfolioServiceClient(channel);
        var productsClient = new ProductGrpc.ProductService.ProductServiceClient(channel);

        var responsePortfolio = await TryGetPortfolio(portfolioClient);
        if (!responsePortfolio.Ok)
        {
            return new GetAllAssetsResponse()
            {
                Error = new PortfolioGrpc.Error()
                {
                    ErrorMessage = responsePortfolio.Exception == null ? "Failed to request" : responsePortfolio.Exception.Message,
                    StackTrace = responsePortfolio.Exception == null ? new Exception().StackTrace : responsePortfolio.Exception.StackTrace
                }
            };
        }

        var responseProducts = await TryGetAllProducts(productsClient);

        var productsInPortfolio = responsePortfolio.Result.AssetArray.Assets.Select(product => product.Id);
        var products = responseProducts.Result.ProductArray.Products.Where(product => productsInPortfolio.Contains(product.Id)).ToList();
        const double ask = 34;
        

        return new GetAllAssetsResponse()
        {
            Error = new PortfolioGrpc.Error()
            {
                ErrorMessage = responseProducts.Exception == null ? "Failed to request" : responseProducts.Exception.Message,
                StackTrace = responseProducts.Exception == null ? new Exception().StackTrace : responseProducts.Exception.StackTrace
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


    private async Task<OperationResult<GetAllAssetsResponse>> TryGetPortfolio(PortfolioGrpc.PortfolioService.PortfolioServiceClient client)
    {
        var result = OperationResult.CreateResult<GetAllAssetsResponse>();

        try
        {
            result.Result = await client.GetAllAssetsAsync(new GetAllAssetsRequest());
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

