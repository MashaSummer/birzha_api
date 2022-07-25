using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class QueryBalanceService: BalanceMicroservice.QueryBalanceService.QueryBalanceServiceBase
    {
        private readonly MongoBalanceService _database;
        private readonly ILogger<QueryBalanceService> _logger;
        public QueryBalanceService(MongoBalanceService mongo, ILogger<QueryBalanceService> logger)
        {
            _database = mongo;
            _logger = logger;
        }

        public override async Task<BalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"User {request.Id} asked for the balance");
            var balanceTask = await _database.GetAsync(new Guid(request.Id));

            return new BalanceResponse
            {
                Balance = balanceTask == null ? 0 : balanceTask.Balance
            };
        }
    }
}