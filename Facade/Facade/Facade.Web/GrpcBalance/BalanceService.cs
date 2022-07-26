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

        public override async Task<BalanceData> GetBalance(EmptyRequest request, ServerCallContext context)
        {

            var result = OperationResult.CreateResult<BalanceData>();
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                _logger.LogError($"invalid id");
                result.AddError("invalid id");
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new QueryBalanceService.QueryBalanceServiceClient(_channel);
                response = await service.GetBalanceAsync(new GetBalanceRequest { Id = id.Value });
                if (response == null || response.Error)
                {
                    _logger.LogError("Bad response : {0}", response.ErrorMessage);
                    result.AddError("Bad response" + response.ErrorMessage);
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
            return result.Exception == null ? new BalanceData { Status = BalanceData.Types.Status.Failed } : result.Result;
        }
        public override async Task<BalanceData> AddBalance(ValueRequest request, ServerCallContext context)
        {

            var result = OperationResult.CreateResult<BalanceData>();
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                _logger.LogError($"Invalid id");
                result.AddError("Invalid id");
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.AddBalanceAsync(new ChangeBalanceRequest { Id = id.Value, Value = request.Value });
                if (response == null || response.Error)
                {
                    _logger.LogError("Bad response : {0}", response.ErrorMessage);
                    result.AddError("Bad response" + response.ErrorMessage);
                }
                else
                {
                    result.Result = new BalanceData { Balance = response.Balance, Status = BalanceData.Types.Status.Success };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Balance method Add {0}", ex.Message);
                result.AddError(ex.Message);
            }
            return result.Exception == null ? new BalanceData { Status = BalanceData.Types.Status.Failed } : result.Result;
        }
        public override async Task<BalanceData> ReduceBalance(ValueRequest request, ServerCallContext context)
        {

            var result = OperationResult.CreateResult<BalanceData>();
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                _logger.LogError($"Invalid id");
                result.AddError("Invalid id");
            }
            BalanceResponse response = new BalanceResponse();
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.ReduseBalanceAsync(new ChangeBalanceRequest { Id = id.Value, Value = request.Value });
                if (response == null || response.Error)
                {
                    _logger.LogError("Bad response : {0}", response.ErrorMessage);
                    result.AddError("Bad response" + response.ErrorMessage);
                }
                else
                {
                    result.Result = new BalanceData { Balance = response.Balance, Status = BalanceData.Types.Status.Success };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error on Balance method reduce {0}", ex.Message);
                result.AddError(ex.Message);
            }
            return result.Exception == null ? new BalanceData { Status = BalanceData.Types.Status.Failed } : result.Result;
        }
    }
}
