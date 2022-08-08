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

        var dbResult = await _repository.GetAllAsync();

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

    public override async Task<ChangePortfolioResponse> AddProduct(ChangePortfolioRequest request, ServerCallContext context)
    {
        var productFromDb = await _repository.Contains(p => p.Name == request.Name);

        if (productFromDb.Ok && productFromDb.Result)
        {
            return new ChangePortfolioResponse()
            {
                Error = new Error()
                {
                    ErrorMessage = "There is product with the same name",
                    StackTrace = ""
                }
            };
        }

        var productModel = _mapper.Map<ProductModel>(request);
        var addingResult = await _repository.AddAsync(productModel);

        if (!addingResult.Ok)
        {
            _logger.LogError($"Error on product service: {addingResult.Error.Message}");
            return new ChangePortfolioResponse()
            {
                Error = new Error()
                {
                    ErrorMessage = addingResult.Error.Message,
                    StackTrace = addingResult.Error.StackTrace
                }
            };
        }
        
        // TODO produce event to Kafka

        return new ChangePortfolioResponse()
        {
            Success = new Success()
            {
                SuccessText = "New product successfully added"
            }
        };
    }
}