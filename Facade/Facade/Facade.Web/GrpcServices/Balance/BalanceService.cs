using BalanceMicroservice;
using Balances;
using Calabonga.OperationResults;
using Facade.Web.Application;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using BalanceMicroservice;

namespace Facade.Web.GrpcServices.Balance
{
    [Authorize]
    public class BalanceService : Balances.Balance.BalanceBase
    {
        private readonly ILogger<BalanceService> _logger;
        
        private readonly ServiceUrls _serviceUrls;
        delegate Task<BalanceResponse> response(BalanceMicroservice.BalanceService.BalanceServiceClient service,string id , ValueRequest value);
        
        public BalanceService(ILogger<BalanceService> logger, IOptionsMonitor<ServiceUrls> optionsMonitor)
        {
            _logger = logger;
            _serviceUrls = optionsMonitor.CurrentValue;
        }


        public override async Task<BalanceData> GetBalance(EmptyRequest request, ServerCallContext context) =>
            (await RequestsToService(
                context,  
                async (service, id, _) => await service.GetBalanceAsync(new GetBalanceRequest { Id = id }))
            ).Result;


        public override async Task<BalanceData> AddBalance(ValueRequest request, ServerCallContext context) =>
            (await RequestsToService(
                context, 
                async (service, id, value) => await service.AddBalanceAsync(new ChangeBalanceRequest { Id = id, Value = value.Value }), 
                request)
            ).Result;


        public override async Task<BalanceData> ReduceBalance(ValueRequest request, ServerCallContext context) => 
            (await RequestsToService(context, 
                async (service, id, value) => await service.ReduseBalanceAsync(new ChangeBalanceRequest { Id = id, Value = value.Value }), 
                request)
            ).Result;


        private async Task<OperationResult<BalanceData>> RequestsToService(
            ServerCallContext context, 
            Func<BalanceServiceProto.BalanceServiceProtoClient, string, ValueRequest, Task<BalanceResponse>> func, 
            ValueRequest request = null)
        {
            var channel = GrpcChannel.ForAddress(_serviceUrls.BalanceService);
            var result = OperationResult.CreateResult<BalanceData>();
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(x => x.Type == "id")!.Value;
            response responseDelegate = new response(func); 

            if (id == null)
            {
                _logger.LogError($"Invalid id");
                result.AddError("Invalid id");
            }
            try
            {
                var service = new BalanceServiceProto.BalanceServiceProtoClient(channel);
                var responseData = await responseDelegate(service, id, request);

                if (responseData == null || responseData.Error)
                {
                    _logger.LogError("Bad response : {0}", responseData.ErrorMessage);
                    result.AddError("Bad response" + responseData.ErrorMessage);
                }
                else
                {
                    result.Result = new BalanceData 
                    {
                        Balance = (double)responseData.BalanceActive / 100,
                        FrozenBalance = (double)responseData.BalanceFrozen / 100,
                        Status = BalanceData.Types.Status.Success 
                    };
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Error on Balance method reduce {0}", ex.Message);
                result.AddError(ex.Message);
            }

            if (!result.Ok)
            {
                return new BalanceData
                {
                    Status = BalanceData.Types.Status.Failed
                };
            }

            return result;
        }
    }
}
