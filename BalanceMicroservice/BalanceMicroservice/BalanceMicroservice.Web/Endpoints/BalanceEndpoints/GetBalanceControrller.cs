namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints
{
    public class GetBalanceController: GetBalanceService.GetBalanceServiceBase
    {
        private readonly BalanceService _database;
        public GetBalanceController(BalanceService mongo)
        {
            _database = mongo;
        }

        public GetBalanceResponse GetBalance(GetBalanceRequest request)
        {
            return new GetBalanceResponse
            {
                Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
            };
        }
    }
}