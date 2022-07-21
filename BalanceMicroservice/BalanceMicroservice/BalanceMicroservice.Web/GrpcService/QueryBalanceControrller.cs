using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class QueryBalanceController: QueryBalanceService.QueryBalanceServiceBase
    {
        private readonly MongoService _database;
        public QueryBalanceController(MongoService mongo)
        {
            _database = mongo;
        }

        public override Task<BalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
        {
            var balanceTask = _database.GetAsync(new Guid(request.Id));

            return Task.FromResult(new BalanceResponse
            {
                Balance = balanceTask == null ? 0 : balanceTask.Result.Balance
            });
        }
    }
}