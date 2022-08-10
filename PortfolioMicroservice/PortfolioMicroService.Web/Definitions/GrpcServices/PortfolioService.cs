using Grpc.Core;
using GrpcServices;
using AutoMapper;
using Calabonga.OperationResults;
using PortfolioMicroService.Definitions.Mongodb.Models;
using PortfolioMicroService.Domain.DbBase;
namespace PortfolioMicroService.Definitions.GrpcServices;

public class PortfolioServices : PortfolioService.PortfolioServiceBase
{
    private readonly IRepository<UserModel> _dbWorker;

    private readonly ILogger<PortfolioServices> _logger;

    private readonly IMapper _mapper;

    PortfolioServices(IRepository<UserModel> dbWorker, ILogger<PortfolioServices> logger, IMapper mapper)
    {
        _dbWorker = dbWorker;
        _logger = logger;
        _mapper = mapper;
    }
    public override Task<GetAllAssetsResponse> GetAllAssets(GetAllAssetsRequest request, ServerCallContext context)
    {
        return null;
    }
}