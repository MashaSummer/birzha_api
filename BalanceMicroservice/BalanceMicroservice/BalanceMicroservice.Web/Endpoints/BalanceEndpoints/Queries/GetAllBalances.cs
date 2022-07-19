using MediatR;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints.Queries
{
    public record GetAllBalancesRequest() : IRequest<string>;

    public class GetAllBalancesRequestHandler : RequestHandler<GetAllBalancesRequest, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BalanceService _balanceService;

        public GetAllBalancesRequestHandler(IHttpContextAccessor httpContextAccessor, BalanceService balanceService)
        {
            _httpContextAccessor = httpContextAccessor;
            _balanceService = balanceService;
        }

        protected override string Handle(GetAllBalancesRequest request)
        {
            var balances = _balanceService.GetAsync();

            return $"Balances: {balances}";
        }
    }
}
