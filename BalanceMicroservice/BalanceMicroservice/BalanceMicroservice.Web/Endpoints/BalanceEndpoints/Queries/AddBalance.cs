using MediatR;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints.Queries
{
    public record PutBalanceRequest(Guid id, int balance) : IRequest<string>;

    public class PutBalanceRequestHandler : RequestHandler<PutBalanceRequest, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BalanceService _balanceService;

        public PutBalanceRequestHandler(IHttpContextAccessor httpContextAccessor, BalanceService balanceService)
        {
            _httpContextAccessor = httpContextAccessor;
            _balanceService = balanceService;
        }

        protected override string Handle(PutBalanceRequest request)
        {
            var balance = _balanceService.GetAsync(request.id).Result;
            balance.Balance += request.balance;
            var balanceResponse = _balanceService.UpdateAsync(request.id, balance);
            return $"Success!";
        }
    }
}
