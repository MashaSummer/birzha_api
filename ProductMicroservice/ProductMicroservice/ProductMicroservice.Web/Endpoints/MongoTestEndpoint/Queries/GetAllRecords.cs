using MediatR;
using ProductMicroservice.Web.DbWorker;
using ProductMicroservice.Web.Endpoints.MongoTestEndpoint.ViewModels;

namespace ProductMicroservice.Web.Endpoints.MongoTestEndpoint.Queries;

public record AllRecordsRequest : IRequest<ProductViewModel[]>;

public class AllRecordsHandler : IRequestHandler<AllRecordsRequest, ProductViewModel[]>
{
    private readonly IDbWorker _dbWorker;

    public AllRecordsHandler(IDbWorker dbWorker) => _dbWorker = dbWorker;

    public async Task<ProductViewModel[]> Handle(AllRecordsRequest request, CancellationToken cancellationToken)
    {
        var result = await _dbWorker.GetAllRecord<ProductViewModel>();

        return result.ToArray();
    }
}