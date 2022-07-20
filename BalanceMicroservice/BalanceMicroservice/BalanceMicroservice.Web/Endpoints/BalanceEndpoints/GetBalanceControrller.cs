namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints
{
    public class GetBalanceController: QueryBalanceService.QueryBalanceServiceBase
    {
        private readonly BalanceService _database;
        public GetBalanceController(BalanceService mongo)
        {
            _database = mongo;
        }

        public BalanceResponse GetBalance(GetBalanceRequest request)
        {
            return new BalanceResponse
            {
                Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
            };
        }
    }
}