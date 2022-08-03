using AutoMapper;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Definitions.Mongodb.ViewModels;
using OrdersMicroservice.Infrastructure.Mongodb;
using OrdersMicroservice.Infrastructure.Mongodb.Context;
using MongoDB.Driver;
using Orders;
using OrdersMicroservice.Domain.DbBase;

namespace OrdersMicroservice.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IMongoDbContext<OrderModel>>(provider => new MongodbContext<OrderModel>(
            new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Orders:Collection"],
                DbName = configuration["Orders:Database"]
            }, provider.GetRequiredService<IMongoClient>()));

        services.AddSingleton<IDbWorker<OrderModel>, MongodbWorker<OrderModel>>();
    }
}