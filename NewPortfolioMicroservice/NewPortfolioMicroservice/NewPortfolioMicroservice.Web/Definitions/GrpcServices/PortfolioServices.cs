using AutoMapper;
using Grpc.Core;
using NewPortfolioMicroservice.Definitions.Mongodb.Models;
using NewPortfolioMicroservice.Domain.DbBase;
using PortfolioMicroService;

namespace NewPortfolioMicroservice.Definitions.GrpcServices;

public class PortfolioServices : PortfolioService.PortfolioServiceBase
{
    private readonly IRepository<UserModel> _dbWorker;

    private readonly ILogger<PortfolioServices> _logger;

    private readonly IMapper _mapper;

    public PortfolioServices(IRepository<UserModel> dbWorker, ILogger<PortfolioServices> logger, IMapper mapper)
    {
        _dbWorker = dbWorker;
        _logger = logger;
        _mapper = mapper;
    }
    public override async Task<GetAllAssetsResponse> GetAllAssets(GetAllAssetsRequest request, ServerCallContext context)
    {
        var getUser = await _dbWorker.GetByIdAsync(request.Guid);

        if(!getUser.Ok)
        {
            return new GetAllAssetsResponse()
            {
                Error = new Error()
                {
                    ErrorMessage = getUser.Error.Message,
                    StackTrace = getUser.Error.StackTrace == null ? "" : getUser.Error.StackTrace
                }
            };
        }
        if (getUser.Result.Assets?.Count() == 0)
        {
            return new GetAllAssetsResponse();
        }
        var response = new GetAllAssetsResponse()
        {
            AssetArray = new AssetArray()
        };

        response.AssetArray.Assets.AddRange(getUser.Result.Assets?.Select(x => _mapper.Map<AssetArray.Types.Asset>(x)));
        _logger.LogDebug("All ok");
        return response;
    }
}