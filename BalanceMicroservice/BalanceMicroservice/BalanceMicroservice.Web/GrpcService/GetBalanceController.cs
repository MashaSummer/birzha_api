using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class GetBalanceController : GetBalanceService.GetBalanceServiceBase
    {
        private readonly MongoController _database;
        public GetBalanceController(MongoController mongo)
        {
            _database = mongo;
        }

        //public GetBalanceResponse GetBalance(GetBalanceRequest request)
        //{
        //    return new GetBalanceResponse
        //    {
        //        Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
        //    };
        //}

        public override Task<GetBalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetBalanceResponse
            {
                Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
            }); ;
        }
    }
}