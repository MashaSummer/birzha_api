using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class CommandBalanceService : BalanceMicroservice.CommandBalanceService.CommandBalanceServiceBase
    {
        private readonly MongoService _database;
        public CommandBalanceService(MongoService mongo)
        {
            _database = mongo;
        }

        public override Task<BalanceResponse> AddBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            return ChangeBalance(request, request.Value);
        }

        public override Task<BalanceResponse> ReduseBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            return ChangeBalance(request, request.Value * -1);
        }



        private async Task<BalanceResponse> ChangeBalance(ChangeBalanceRequest request, double value)
        {
            await _database.UpdateAsync(CalculateNewBalance(await GetBalance(request.Id), value));

            return new BalanceResponse
            {
                Balance = (await _database.GetAsync(new Guid(request.Id))).Balance
            };
        }


        private static BalanceViewModel CalculateNewBalance(BalanceViewModel oldValue, double newValue)
        {
            return new BalanceViewModel {
                Id = oldValue.Id,
                Balance = oldValue.Balance + newValue
            };
        }

        // Нужно ли запрвшивать баланс по gRPC у отдающей части, что бы соблюсти cqrs или и так нормально?
        private async Task<BalanceViewModel> GetBalance(string id)
        {
            Guid userId = new(id);

            var balanceTask = _database.GetAsync(userId);

            if (await balanceTask == null)
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
                        Id = userId,
                        Balance = 0
                    });
                balanceTask = _database.GetAsync(userId);
            }

            return await balanceTask;
        }
    }
}