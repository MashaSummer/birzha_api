using Calabonga.OperationResults;
using Facade.Web.Application;
using Facade.Web.GrpcServices.Portfolio.Aggregation;
using Facade.Web.GrpcServices.Product;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using PortfolioGrpc;
using PortfolioServiceGrpc;
using ProductGrpc;
using Orders;
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
        var channelOrders = GrpcChannel.ForAddress(_serviceUrls.OrdersService);

        var portfolioClient = new PortfolioGrpc.PortfolioService.PortfolioServiceClient(channelPortfolio);
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(channelProduct);
        var ordersClient = new Orders.OrdersService.OrdersServiceClient(channelOrders);

        var responsePortfolio = await TryGetPortfolio(portfolioClient, productClient, ordersClient, context);

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
        ProductGrpc.ProductService.ProductServiceClient productClient,
        Orders.OrdersService.OrdersServiceClient ordersClient,
        ServerCallContext context)
    {
        var responsePortfolio = await TryGetAssets(context, portfolioClient);
        var assetsArray = responsePortfolio.Result.AssetArray.Assets;

        var responseProduct = await TryGetAllProducts(productClient);
        var productsArray = responseProduct.Result.ProductArray.Products;

        var responseOrders = await TryGetOrders(context, ordersClient, productsArray);
        var userProductsInfo = responseOrders.Result.Success.Products;

        var result = OperationResult.CreateResult<GetPortfolioResponse>();

        GetPortfolioResponse portfolio = new GetPortfolioResponse();
        try
        {
            var productsInPortfolio = PortfolioAggregator.AggregateProducts(portfolio, assetsArray, productsArray, userProductsInfo).Portfolio.Products;
            result.Result = PortfolioAggregator.AggregateTotal(portfolio, productsInPortfolio);
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

    private async Task<OperationResult<UserProductsResponse>> TryGetOrders(ServerCallContext context, Orders.OrdersService.OrdersServiceClient client,
        Google.Protobuf.Collections.RepeatedField<ProductArray.Types.Product> productsArray)
    {
        var result = OperationResult.CreateResult<UserProductsResponse>();
        var id = context.GetHttpContext().User.Claims.FirstOrDefault(claim => claim.Type == "id");
        
        try
        {
            UserProductsRequest userProductsRequest = new UserProductsRequest();

            userProductsRequest.InvestorId = id.Value;
            foreach (var product in productsArray) 
            { 
                userProductsRequest.ProductsId.Add(product.Id); 
            }
           

            result.Result = await client.GetUserProductsInfoAsync(userProductsRequest);
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

