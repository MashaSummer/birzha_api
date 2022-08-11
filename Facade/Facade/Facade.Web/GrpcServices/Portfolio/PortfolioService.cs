using Calabonga.OperationResults;
using Facade.Web.Application;
using Facade.Web.GrpcServices.Product;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using PortfolioGrpc;
using PortfolioServiceGrpc;
using ProductGrpc;
using static OpenIddict.Abstractions.OpenIddictConstants;

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

    public override async Task<PortfolioServiceGrpc.GetPortfolioResponse> GetPortfolio(PortfolioServiceGrpc.GetPortfolioRequest request, ServerCallContext context)
    {
        var channelPortfolio = GrpcChannel.ForAddress(_serviceUrls.PortfolioService);
        var channelProduct = GrpcChannel.ForAddress(_serviceUrls.ProductService);

        var portfolioClient = new PortfolioGrpc.PortfolioService.PortfolioServiceClient(channelPortfolio);
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(channelProduct);

        var responsePortfolio = await TryGetPortfolio(portfolioClient, productClient, context);

        if (responsePortfolio.Ok)
        {
            return responsePortfolio.Result;
        }

        return new GetPortfolioResponse()
        {
            Error = new PortfolioServiceGrpc.Error()
            {
                ErrorMessage = responsePortfolio.Exception == null ? "Failed to request" : responsePortfolio.Exception.Message,
                StackTrace = responsePortfolio.Exception == null ? new Exception().StackTrace : responsePortfolio.Exception.StackTrace
            }
        };
    }

    private async Task<OperationResult<GetPortfolioResponse>> TryGetPortfolio(
        PortfolioGrpc.PortfolioService.PortfolioServiceClient portfolioClient, 
        ProductGrpc.ProductService.ProductServiceClient productClient, ServerCallContext context)
    {
        var responsePortfolio = await TryGetAssets(context, portfolioClient);
        var responseProduct = await TryGetAllProducts(productClient);
        var result = OperationResult.CreateResult<GetPortfolioResponse>();

        var assetsArray = responsePortfolio.Result.AssetArray.Assets;
        var productsArray = responseProduct.Result.ProductArray.Products;
        GetPortfolioResponse portfolioResponse = new GetPortfolioResponse();
        try
        {
            double estimate, spent, earned, delta_abs, delta_rel;
            for (int i = 0; i < assetsArray.Count(); i++)
            {
                var product = productsArray.Where(p => p.Id == assetsArray[i].Id).First();
                var asset = assetsArray[i];
                estimate = asset.VolumeActive * product.BestAsk;
                spent = 50;
                earned = 50;
                delta_abs = estimate - spent;
                delta_rel = (estimate - spent) / spent;
                portfolioResponse.Portfolio.Products.Add(new PortfolioServiceGrpc.Portfolio.Types.Product
                {
                    Id = asset.Id,
                    Name = product.Name,
                    BestAsk = product.BestAsk,
                    Spent = spent,
                    Earned = earned,
                    Volume = asset.VolumeActive,
                    Estimate = estimate,
                    DeltaAbs = delta_abs,
                    DeltaRel = delta_rel
                });
            }
            var portfolio = portfolioResponse.Portfolio.Products;
            portfolioResponse.Portfolio.Total = new PortfolioServiceGrpc.Portfolio.Types.Total
            {
                Spent = portfolio.Sum(sp => sp.Spent),
                Earned = portfolio.Sum(sp => sp.Earned),
                Estimate = portfolio.Sum(sp => sp.Estimate),
                DeltaAbs = portfolio.Sum(sp => sp.DeltaAbs),
                DeltaRel = portfolio.Sum(sp => sp.DeltaRel)
            };

            result.Result = portfolioResponse;
            if (result.Result == null)
            {
                result.AddError(new Exception("Failed to request"));
            }
        }
        catch(Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
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

    private async Task<OperationResult<GetAllAssetsResponse>> TryGetAssets(ServerCallContext context, PortfolioGrpc.PortfolioService.PortfolioServiceClient client)
    {
        var result = OperationResult.CreateResult<GetAllAssetsResponse>();
        var id = context.GetHttpContext().User.Claims.FirstOrDefault(claim => claim.Type == "id");
        
        try
        {
            result.Result = await client.GetAllAssetsAsync(new GetAllAssetsRequest { Id = id.Value });
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

