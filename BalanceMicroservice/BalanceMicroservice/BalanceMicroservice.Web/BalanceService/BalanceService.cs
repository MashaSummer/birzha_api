using BalanceMicroservice.Web.MongoService;
using Grpc.Core;
using BalanceMicroservice.Web.MongoService.ViewModels;

namespace BalanceMicroservice.Web.BalanceService
{
    public class BalanceService : BalanceMicroservice.BalanceService.BalanceServiceBase
    {
        private readonly MongoBalanceService _database;
        private readonly ILogger<BalanceService> _logger;
        public BalanceService(MongoBalanceService mongo, ILogger<BalanceService> logger)
        {
            _database = mongo;
            _logger = logger;
        }

        public override async Task<BalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation("User {0} asked for the balance", request.Id);
            var balance = await _database.GetByIdAsync(new Guid(request.Id));

            return new BalanceResponse
            {
                BalanceActive = balance == null ? 0 : (int)(balance.BalanceActive * 100),
                BalanceFrozen = balance == null ? 0 : (int)(balance.BalanceFrozen * 100)
            };
        }

        public override Task<BalanceResponse> AddBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Add balance request for user {0} in the amount of {1} units", request.Id, request.Value);
            return ChangeBalance(request.Id, request.Value, false);
        }

        public override Task<BalanceResponse> ReduseBalance(ChangeBalanceRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Reduse balance request for user {0} in the amount of {1} units", request.Id, request.Value);
            return ChangeBalance(request.Id, request.Value, true);
        }



        private async Task<BalanceResponse> ChangeBalance(string id, decimal value, bool negative)
        {
            if (value <= 0)
            {
                _logger.LogError("User {0} tried to use negative value ({1})", id, value);
                return new BalanceResponse
                {
                    Error = true,
                    ErrorMessage = "Error: negative or zero value"
                };
            }

            if (negative) { value *= -1; }

            await _database.UpdateAsync(CalculateNewBalance(await GetBalance(id), value));
            var balance = await _database.GetByIdAsync(new Guid(id));

            return new BalanceResponse
            {
                BalanceActive = balance == null ? 0 : (int)(balance.BalanceActive * 100),
                BalanceFrozen = balance == null ? 0 : (int)(balance.BalanceFrozen * 100)
            };
        }


        private static BalanceViewModel CalculateNewBalance(BalanceViewModel oldValue, decimal newValue)
        {
            return new BalanceViewModel
            {
                Id = oldValue.Id,
                BalanceActive = oldValue.BalanceActive + newValue,
                BalanceFrozen = oldValue.BalanceFrozen,
            };
        }

        // Нужно ли запрвшивать баланс по gRPC у отдающей части, что бы соблюсти cqrs или и так нормально?
        private async Task<BalanceViewModel> GetBalance(string id)
        {
            Guid userId = new(id);

            var balance = await _database.GetByIdAsync(userId);

            if (balance == null)
            {
                _logger.LogInformation("User {0} has no record", id);
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
                        BalanceActive = 0,
                        BalanceFrozen = 0,
                    });
                _logger.LogInformation("Record for user {0} successfully created", id);
                balance = await _database.GetByIdAsync(userId);
            }

            return balance;
        }
    }
}