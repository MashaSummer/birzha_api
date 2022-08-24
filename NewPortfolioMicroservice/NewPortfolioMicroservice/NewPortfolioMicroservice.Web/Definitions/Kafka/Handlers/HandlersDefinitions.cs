using Confluent.Kafka;
using GrpcServices;
using NewPortfolioMicroservice.Definitions.Base;
using NewPortfolioMicroservice.Definitions.Kafka.Models;
using NewPortfolioMicroservice.Domain.EventsBase;
using PortfolioMicroService;

namespace NewPortfolioMicroservice.Definitions.Kafka.Handlers;

public class HandlersDefinitions : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventHandler<Null, ProductAddEvent>, AddProductHandler>();
        services.AddTransient<IEventHandler<Null, UserRegisterEvent>, AuthRegisterHandler>();
        services.AddTransient<IEventHandler<Null, OrderExecuteEvent>, OrdersExecuteHandler>();
        services.AddTransient<IEventHandler<Null, OrderCreatedEvent>, OrdersCreatedHandler>();
    }
}