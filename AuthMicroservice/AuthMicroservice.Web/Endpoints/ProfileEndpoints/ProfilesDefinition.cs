using AuthMicroservice.Web.Application;
using AuthMicroservice.Web.Definitions.Base;
using AuthMicroservice.Web.Definitions.OpenIddict;
using AuthMicroservice.Web.Endpoints.ProfileEndpoints.Queries;
using AuthMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Calabonga.OperationResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthMicroservice.Web.Endpoints.ProfileEndpoints;

public class ProfilesDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        app.MapGet("/api/profiles/get-roles", GetRoles);
        app.MapPost("/api/profiles/register", RegisterAccount);
    }
    
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [Authorize(AuthenticationSchemes = AuthData.AuthSchemes)]
    [FeatureGroupName("Profiles")]
    private async Task<string> GetRoles([FromServices] IMediator mediator, HttpContext context)
        => await mediator.Send(new GetRolesRequest(), context.RequestAborted);

    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [FeatureGroupName("Profiles")]
    private async Task<OperationResult<UserProfileViewModel>> RegisterAccount([FromServices] IMediator mediator, RegisterViewModel model, HttpContext context)
        => await mediator.Send(new RegisterAccountRequest(model), context.RequestAborted);
}