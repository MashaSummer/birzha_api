using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductMicroservice.Web.Definitions.Base;
using ProductMicroservice.Web.Definitions.Identity;
using ProductMicroservice.Web.Endpoints.ProfileEndpoints.Queries;

namespace ProductMicroservice.Web.Endpoints.ProfileEndpoints
{
    public class ProfilesEndpointDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
            => app.MapGet("/api/profiles/get-roles", GetRoles);

        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize(AuthenticationSchemes = AuthData.AuthSchemes, Policy = "EventItems:UserRoles:View")]
        [FeatureGroupName("Profiles")]
        private async Task<string> GetRoles([FromServices] IMediator mediator, HttpContext context)
            => await mediator.Send(new GetRolesRequest(), context.RequestAborted);
    }
}