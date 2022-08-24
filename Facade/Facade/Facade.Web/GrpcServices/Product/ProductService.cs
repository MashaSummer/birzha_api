using AutoMapper;
using Calabonga.OperationResults;
using Facade.Web.Application;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Orders;
using ProductGrpc;
using Error = ProductGrpc.Error;

namespace Facade.Web.GrpcServices.Product;

[Authorize]
public class ProductService : ProductGrpc.ProductService.ProductServiceBase
{
    private readonly ILogger<ProductService> _logger;
    private readonly ServiceUrls _serviceUrls;
    
    public ProductService(
        ILogger<ProductService> logger,
        IOptionsMonitor<ServiceUrls> optionsMonitor)
    {
        _logger = logger;
        _serviceUrls = optionsMonitor.CurrentValue;
    }

    public override async Task<ChangePortfolioResponse> AddProduct(ChangePortfolioRequest request, ServerCallContext context)
    {

        //Validation for negative volume of products
        var addProductResponse = OperationResult.CreateResult<ChangePortfolioResponse>();
        if(request.Volume <= 0)
        {
            return HandleErrorForAdd(addProductResponse, "Failed to request, check your volume of products");
        }

        // Creating channel and client for request to product microservice
        var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = 
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var productChannel = GrpcChannel.ForAddress(_serviceUrls.ProductService, new GrpcChannelOptions { HttpHandler = httpHandler });
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(productChannel);

        // Handling response from product microservice
        addProductResponse = await TryAddProduct(request , productClient, context);
        if (!addProductResponse.Ok)
        {
            return HandleErrorForAdd(addProductResponse, "Failed to request to product microservice");
        }

        return addProductResponse.Result;
    }

    public override async Task<GetAllProductsResponse> GetAllProducts(
        GetAllProductsRequest request, 
        ServerCallContext context)
    {
        var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = 
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    
        // Creating channel and client for request to product microservice
        var productChannel = GrpcChannel.ForAddress(_serviceUrls.ProductService, new GrpcChannelOptions { HttpHandler = httpHandler });
        var productClient = new ProductGrpc.ProductService.ProductServiceClient(productChannel);

        
        // Creating channel and client for request to orders microservice
        var orderChannel = GrpcChannel.ForAddress(_serviceUrls.OrdersService, new GrpcChannelOptions { HttpHandler = httpHandler });
        var orderClient = new OrdersService.OrdersServiceClient(orderChannel);

        
        // Handling response from product microservice
        var productResponse = await TryGetAllProducts(productClient);
        if (!productResponse.Ok)
        {
            return HandleError(productResponse, "Failed to request to product microservice");
        }
        
        
        // Handling response from orders microservice
        var ordersResponse = await TryGetProductsInfo(orderClient, productResponse.Result.Select(p => p.Id));
        if (!ordersResponse.Ok)
        {
            return HandleError(ordersResponse, "Failed to request to orders microservice");
        }

        
        // Join two collection
        var products = ordersResponse.Result
            .Where(info => info.BestAsk != -1)
            .Select(info => new ProductArray.Types.Product()
            {
                Id = info.ProductId, 
                Name = productResponse.Result.First(p => p.Id == info.ProductId).Name, 
                BestAsk = info.BestAsk, 
                BestBid = info.BestBid
            });

        var response = new GetAllProductsResponse();
        response.ProductArray.Products.AddRange(products);

        return response;
    }


    private async Task<OperationResult<IEnumerable<ProductArray.Types.Product>>> TryGetAllProducts(ProductGrpc.ProductService.ProductServiceClient client)
    {
        var result = OperationResult.CreateResult<IEnumerable<ProductArray.Types.Product>>();

        try
        {
            var response = await client.GetAllProductsAsync(new GetAllProductsRequest());
            if (response == null || response.Error != null)
            {
                result.AddError(new Exception("Failed to request"));
                return result;
            }

            result.Result = response.ProductArray.Products.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    private async Task<OperationResult<IEnumerable<ProductInfo>>> TryGetProductsInfo(OrdersService.OrdersServiceClient client, IEnumerable<string> ids)
    {
        var result = OperationResult.CreateResult<IEnumerable<ProductInfo>>();

        try
        {
            var request = new ProductInfoRequest();
            request.ProductsId.AddRange(ids);
            var response = await client.GetProductsInfoAsync(request);
            if (response == null || response.Error != null)
            {
                result.AddError(new Exception("Failed to request"));
                return result;
            }

            result.Result = response.Success.ProductsInfo.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    private async Task<OperationResult<ChangePortfolioResponse>> TryAddProduct(ChangePortfolioRequest request, ProductGrpc.ProductService.ProductServiceClient client,
        ServerCallContext context)
    {
        var id = context.GetHttpContext().User.Claims.FirstOrDefault(claim => claim.Type == "id");
        var result = OperationResult.CreateResult<ChangePortfolioResponse>();

        try
        {
            result.Result = await client.AddProductAsync(new ChangePortfolioRequest
            {
                InvestorId = id.Value,
                ProductName = request.ProductName,
                StartPrice = request.StartPrice,
                Volume = request.Volume
            });
            if(result.Result == null)
            {
                result.AddError(new Exception("Failed to request!"));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }
        return result;
    }

    private GetAllProductsResponse HandleError(OperationResult result, string alternativeText) => new GetAllProductsResponse()
    {
        Error = new Error()
        {
            ErrorMessage = result?.Exception?.Message ?? alternativeText, 
            StackTrace = result?.Exception?.StackTrace ?? "No stack trace"
        }
    };
    private ChangePortfolioResponse HandleErrorForAdd(OperationResult result, string alternativeText) => new ChangePortfolioResponse()
    {
        Error = new Error()
        {
            ErrorMessage = result?.Exception?.Message ?? alternativeText,
            StackTrace = result?.Exception?.StackTrace ?? new Exception().StackTrace
        }
    };
}

