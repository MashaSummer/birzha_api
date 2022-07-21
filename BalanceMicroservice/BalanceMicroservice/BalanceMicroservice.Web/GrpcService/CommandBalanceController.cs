using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class CommandBalanceController : CommandBalanceService.CommandBalanceServiceBase
    {
        private readonly MongoService _database;
        public CommandBalanceController(MongoService mongo)
        {
            _database = mongo;
        }

        public override async Task<BalanceResponse> AddBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            BalanceViewModel balanceOld = await CheckBalance(request.Id);

            await _database.UpdateAsync(CalculateNewBalance(balanceOld, request.Value));
            
            return new BalanceResponse
            {
                Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
            };
        }

        public override async Task<BalanceResponse> ReduseBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            BalanceViewModel balanceOld = await CheckBalance(request.Id);

            await _database.UpdateAsync(CalculateNewBalance(balanceOld, request.Value * -1));

            return new BalanceResponse
            {
                Balance = _database.GetAsync(new Guid(request.Id)).Result.Balance
            };
        }


        private static BalanceViewModel CalculateNewBalance(BalanceViewModel oldValue, double newValue)
        {
            return new BalanceViewModel {
                Id = oldValue.Id,
                Balance = oldValue.Balance + newValue
            };
        }

        private async Task<BalanceViewModel> CheckBalance(string id)
        {
            var balanceTask = _database.GetAsync(new Guid(id));

            if (balanceTask == null)
            {
                /*return new BalanceResponse
                {
                    Balance = 0,
                    Error = true,
                    ErrorMessage = "User does not exists"
                };*/

                await _database.CreateAsync(
                    new BalanceViewModel
                    {
                        Id = new Guid(id),
                        Balance = 0
                    });
                balanceTask = _database.GetAsync(new Guid(id));
            }
            return await balanceTask;
        }
    }
}