using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductMicroservice.Web.Definitions.Base;
using ProductMicroservice.Web.Endpoints.MongoTestEndpoint.Queries;
using ProductMicroservice.Web.Endpoints.MongoTestEndpoint.ViewModels;

namespace ProductMicroservice.Web.Endpoints.MongoTestEndpoint;

public class MongoTestEndpoint : AppDefinition
{
    public override void ConfigureApplication(WebApplication app, IWebHostEnvironment env) => 
        app.MapGet("/get/records/all", GetAllRecords);

    private async Task<ProductViewModel[]> GetAllRecords([FromServices] IMediator mediator, HttpContext context) => 
        await mediator.Send(new AllRecordsRequest(), context.RequestAborted);
}