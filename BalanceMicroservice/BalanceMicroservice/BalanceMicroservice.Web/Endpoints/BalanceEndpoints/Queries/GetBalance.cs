using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using Calabonga.Microservices.Core;
using MediatR;
using System.Security.Claims;

namespace BalanceMicroservice.Web.Endpoints.ProfileEndpoints.Queries
{
    /// <summary>
    /// Request: Returns user roles 
    /// </summary>
    public record GetBalanceRequest(Guid id) : IRequest<string>;

    public class GetBalanceRequestHandler : RequestHandler<GetBalanceRequest, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BalanceService _balanceService;

        public GetBalanceRequestHandler(IHttpContextAccessor httpContextAccessor, BalanceService balanceService)
        {
            _httpContextAccessor = httpContextAccessor;
            _balanceService = balanceService;
        } 

        protected override string Handle(GetBalanceRequest request)
        {
            var balance = _balanceService.GetAsync(request.id);
            
            return $"Current user ({request.id}) have following balance: {string.Join("|", balance)}";
        }
    }
}