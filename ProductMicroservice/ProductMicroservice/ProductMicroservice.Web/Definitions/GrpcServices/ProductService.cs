using AutoMapper;
using Google.Protobuf.Collections;
using Grpc.Core;
using ProductGrpc;
using ProductMicroservice.Definitions.Mongodb.Models;
using ProductMicroservice.Domain.DbBase;
using ILogger = Grpc.Core.Logging.ILogger;

namespace ProductMicroservice.Definitions.GrpcServices;

public class ProductService : ProductGrpc.ProductService.ProductServiceBase
{
    private readonly IDbWorker<ProductModel> _dbWorker;

    private readonly ILogger<ProductService> _logger;

    private readonly IMapper _mapper;
    
    public ProductService(IDbWorker<ProductModel> dbWorker, ILogger<ProductService> logger, IMapper mapper)
    {
        _dbWorker = dbWorker;
        _logger = logger;
        _mapper = mapper;
    }

    public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Get all data from database");
        
        var productModels = await _dbWorker.GetAllRecords();
        
        foreach (var productModel in productModels)
        {
            Console.WriteLine($"{productModel.Id} {productModel.Name} {productModel.BestAsk} {productModel.BestBid}");
        }

        var result = new GetAllProductsResponse()
        {
            ProductArray = new ProductArray()
        };
        
        result.ProductArray.Products.AddRange(productModels.Select(x => 
                _mapper.Map<ProductArray.Types.Product>(x)).ToList());

        return result;
    }
}