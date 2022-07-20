using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using MediatR;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints.Queries
{
    public record GetAllBalancesRequest() : IRequest<string>;

    public class GetAllBalancesRequestHandler : RequestHandler<GetAllBalancesRequest, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MongoController _balanceService;

        public GetAllBalancesRequestHandler(IHttpContextAccessor httpContextAccessor, MongoController balanceService)
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
