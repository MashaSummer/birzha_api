using MediatR;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints.Queries
{
    public record ReduceBalanceRequest(Guid id, int balance) : IRequest<string>;

    public class ReduceBalanceRequestHandler : RequestHandler<ReduceBalanceRequest, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BalanceService _balanceService;

        public ReduceBalanceRequestHandler(IHttpContextAccessor httpContextAccessor, BalanceService balanceService)
        {
            _httpContextAccessor = httpContextAccessor;
            _balanceService = balanceService;
        }

        protected override string Handle(ReduceBalanceRequest request)
        {
            var balance = _balanceService.GetAsync(request.id).Result;
            balance.Balance -= request.balance;
            var balanceResponse = _balanceService.UpdateAsync(request.id, balance);
            return $"Success!";
        }
    }
}
