using Grpc.Core;
using Balances;
using BalanceMicroservice;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;

namespace Facade.Web.GrpcBalance
{
    public class BalanceService : Balance.BalanceBase
    {
        private readonly ILogger<BalanceService> _logger;
        private readonly GrpcChannel _channel;
        public BalanceService(ILogger<BalanceService> logger,GrpcChannel channel)
        {
            _logger = logger;
            _channel = channel;
        }
       [Authorize]
        public override async  Task<BalanceData> GetBalance(EmptyRequest request, ServerCallContext context)
        {
            var hasError = false;
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if(id == null)
            {
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            BalanceResponse response = null;
            try
            {
                var service = new QueryBalanceService.QueryBalanceServiceClient(_channel);
                response = await service.GetBalanceAsync(new GetBalanceRequest { Id = id.ToString()});
            } 
            catch(Exception ex)
            {
                _logger.LogError($"Error on Balance method get: {ex.Message}");
                hasError = true;
            }
            return await Task.FromResult(new BalanceData() { 
                Balance = response.Balance,
                Status = hasError ? BalanceData.Types.Status.Success : BalanceData.Types.Status.Failed
            });
        }
        [Authorize]
        public override async Task<BalanceData> AddBalance(ValueRequest request, ServerCallContext context)
        {
            var hasError = false;
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            BalanceResponse response = null;
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.AddBalanceAsync(new ChangeBalanceRequest { Id = id.ToString(), Value = request.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on Balance method Add: {ex.Message}");
                hasError = true;
            }
            return await Task.FromResult(new BalanceData()
            {
                Balance = response.Balance,
                Status = hasError ? BalanceData.Types.Status.Success : BalanceData.Types.Status.Failed
            });
        }
        [Authorize]
        public override async Task<BalanceData> ReduceBalance(ValueRequest request, ServerCallContext context)
        {
            var hasError = false;
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id");
            if (id == null)
            {
                return await Task.FromResult(new BalanceData() { Balance = 0, Status = BalanceData.Types.Status.Failed });
            }
            BalanceResponse response = null;
            try
            {
                var service = new CommandBalanceService.CommandBalanceServiceClient(_channel);
                response = await service.ReduseBalanceAsync(new ChangeBalanceRequest { Id = id.ToString(), Value = request.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on Balance method reduce: {ex.Message}");
                hasError = true;
            }
            return await Task.FromResult(new BalanceData()
            {
                Balance = response.Balance,
                Status = hasError ? BalanceData.Types.Status.Success : BalanceData.Types.Status.Failed
            });
        }
    }
}
