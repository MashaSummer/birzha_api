using Grpc.Core;
using Balances;
using BalanceMicroservice;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Calabonga.OperationResults;

namespace Facade.Web.GrpcBalance
{
    [Authorize]
    public class BalanceService : Balance.BalanceBase
    {
        private readonly ILogger<BalanceService> _logger;
        private readonly GrpcChannel _channel;
        public BalanceService(ILogger<BalanceService> logger, GrpcChannel channel)
        {
            _logger = logger;
            _channel = channel;
        }

        [AllowAnonymous]
        public override async Task<BalanceData> GetBalance(EmptyRequest request, ServerCallContext context)
        {
            var hasError = false;
            var result = OperationResult.CreateResult<BalanceData>();
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                _logger.LogError($"Invalid id");
                result.AddError("Invalid id");
                return result.Result;
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new QueryBalanceService.QueryBalanceServiceClient(_channel);
                response = await service.GetBalanceAsync(new GetBalanceRequest { Id = id.Value });
                if (response == null)
                {
                    _logger.LogError("Bad response");
                    result.AddError("Bad response");
                }
                else
                {
                    result.Result = new BalanceData { Balance = response.Balance, Status = BalanceData.Types.Status.Success };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Balance method get {0}", ex.Message);
                result.AddError(ex.Message);
            }

            return result.Result;
        }

        public override async Task<BalanceData> AddBalance(ValueRequest request, ServerCallContext context)
        {
            var hasError = false;
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.AddBalanceAsync(new ChangeBalanceRequest { Id = id.Value, Value = request.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Balance method Add {0}", ex.Message);
                hasError = true;
            }
            if (response == null)
            {
                _logger.LogError("Bad response");
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            return await Task.FromResult(new BalanceData()
            {
                Balance = response.Balance,
                Status = hasError ? BalanceData.Types.Status.Failed : BalanceData.Types.Status.Success
            });
        }

        public override async Task<BalanceData> ReduceBalance(ValueRequest request, ServerCallContext context)
        {
            var hasError = false;
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.ReduseBalanceAsync(new ChangeBalanceRequest { Id = id.Value, Value = request.Value });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Balance method reduce {0}", ex.Message);
                hasError = true;
            }
            if (response == null)
            {
                _logger.LogError("Bad response");
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            return await Task.FromResult(new BalanceData()
            {
                Balance = response.Balance,
                Status = hasError ? BalanceData.Types.Status.Failed : BalanceData.Types.Status.Success
            });
        }


    }
}
