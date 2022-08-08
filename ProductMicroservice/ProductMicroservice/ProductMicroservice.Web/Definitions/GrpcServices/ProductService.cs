using AutoMapper;
using Calabonga.OperationResults;
using Grpc.Core;
using ProductGrpc;
using ProductMicroservice.Domain.DbBase;
using ProductMicroservice.Mongodb.Models;

namespace ProductMicroservice.Definitions.GrpcServices;

public class ProductService : ProductGrpc.ProductService.ProductServiceBase
{
    private readonly IRepository<ProductModel> _repository;

    private readonly ILogger<ProductService> _logger;

    private readonly IMapper _mapper;
    
    public ProductService(IRepository<ProductModel> repository, ILogger<ProductService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Get all data from database");

        var dbResult = await GetAllProductsFromDb();

        if (!dbResult.Ok)
        {
            return new GetAllProductsResponse()
            {
                Error = new Error()
                {
                    ErrorMessage = dbResult.Error.Message,
                    StackTrace = dbResult.Error.StackTrace
                }
            };
        }
        
        var productModels = dbResult.Result;

        var response = new GetAllProductsResponse()
        {
            ProductArray = new ProductArray()
        };

        response.ProductArray.Products.AddRange(productModels.Select(x =>
            _mapper.Map<ProductArray.Types.Product>(x)).ToList());
        
        return response;
    }

    private async Task<OperationResult<IEnumerable<ProductModel>>> GetAllProductsFromDb()
    {
        OperationResult<IEnumerable<ProductModel>> result = new OperationResult<IEnumerable<ProductModel>>();
        try
        {
            result.Result = await _repository.GetAllAsync();
            if (result.Result == null)
            {
                result.AddError(new Exception("Failed to get products data from database"));
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