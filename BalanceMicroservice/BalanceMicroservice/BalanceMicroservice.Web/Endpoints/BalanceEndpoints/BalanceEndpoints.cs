using BalanceMicroservice.Web.Definitions.Base;
using BalanceMicroservice.Web.Definitions.Identity;
using BalanceMicroservice.Web.Endpoints.BalanceEndpoints.Queries;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BalanceMicroservice.Web.Endpoints.ProfileEndpoints
{
    public class BalanceEndpointDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
        {
            app.MapGet("/api/balance/get-balance/{id}", GetBalance);
            //app.MapGet("/api/balance/get-balances", GetAllBalances);
            app.MapPut("/api/balance/add-balance/{id}", AddBalance);
            app.MapPut("/api/balance/reduce-balance/{id}", ReduceBalance);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [FeatureGroupName("Balance")]
        private async Task<string> GetBalance([FromServices] IMediator mediator, Guid id, HttpContext context)
            => await mediator.Send(new GetBalanceRequest(id), context.RequestAborted);

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [FeatureGroupName("Balance")]
        private async Task<string> GetAllBalances([FromServices] IMediator mediator, HttpContext context)
           => await mediator.Send(new GetAllBalancesRequest(), context.RequestAborted);

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [FeatureGroupName("Balance")]
        private async Task<string> ReduceBalance([FromServices] IMediator mediator, Guid id, int balance, HttpContext context)
           => await mediator.Send(new ReduceBalanceRequest(id, balance), context.RequestAborted);

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [FeatureGroupName("Balance")]
        private async Task<string> AddBalance([FromServices] IMediator mediator, Guid id, int balance, HttpContext context)
           => await mediator.Send(new PutBalanceRequest(id, balance), context.RequestAborted);
    }
}