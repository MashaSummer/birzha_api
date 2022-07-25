using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Grpc.Core;

namespace BalanceMicroservice.Web.GrpcService
{
    public class CommandBalanceService : BalanceMicroservice.CommandBalanceService.CommandBalanceServiceBase
    {
        private readonly MongoBalanceService _database;
        private readonly ILogger<CommandBalanceService> _logger;
        public CommandBalanceService(MongoBalanceService mongo, ILogger<CommandBalanceService> logger)
        {
            _database = mongo;
            _logger = logger;
        }

        public override Task<BalanceResponse> AddBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Add balance request for user {request.Id} in the amount of {request.Value} units");
            return ChangeBalance(request.Id, request.Value, false);
        }

        public override Task<BalanceResponse> ReduseBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Reduse balance request for user {request.Id} in the amount of {request.Value} units");
            return ChangeBalance(request.Id, request.Value, true);
        }



        private async Task<BalanceResponse> ChangeBalance(string id, double value, bool negative)
        {
            if (value <= 0) 
            {
                _logger.LogError($"User {id} tried to use negative value ({value})");
                return new BalanceResponse
                {
                    Balance = 0,
                    Error = true,
                    ErrorMessage = "You can`t use negative or zero value"
                };
            }

            if (negative) { value *= -1; }

            await _database.UpdateAsync(CalculateNewBalance(await GetBalance(id), value));

            return new BalanceResponse
            {
                Balance = (await _database.GetAsync(new Guid(id))).Balance
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

            var balance = await _database.GetAsync(userId);

            if (balance == null)
            {
                _logger.LogInformation($"User {id} has no record");
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
                _logger.LogInformation($"Record for user {id} successfully created");
                balance = await _database.GetAsync(userId);
            }

            return balance;
        }
    }
}