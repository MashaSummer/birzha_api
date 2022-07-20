using BalanceMicroservice.Web.Endpoints.BalanceEndpoints;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Calabonga.Microservices.Core;
using MediatR;
using MongoDB.Bson;
using System.Security.Claims;

namespace BalanceMicroservice.Web.Endpoints.ProfileEndpoints.Queries
{
    /// <summary>
    /// Request: Returns user roles 
    /// </summary>
    //public record GetBalanceRequest(Guid id) : IRequest<string>;

    //public class GetBalanceRequestHandler : RequestHandler<GetBalanceRequest, string>
    //{
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly MongoConttoller _balanceService;

    //    public GetBalanceRequestHandler(IHttpContextAccessor httpContextAccessor, MongoConttoller balanceService)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //        _balanceService = balanceService;
    //    } 

    //    protected override string Handle(GetBalanceRequest request)
    //    {
    //        GetBalanceController getBalanceController = new GetBalanceController(_balanceService);
    //        var balance = getBalanceController.GetBalance(request);
    //        return $"Current user ({request.id}) have following balance: {string.Join("|", balance.Balance)}";
    //    }
    //}
}