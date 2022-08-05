using AutoMapper;
using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;
using MongoDB.Driver;
using OrdersMicroservice.Domain.DbBase;

namespace OrdersMicroservice.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IRepository<OrderModel>>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Orders:Collection"],
                DbName = configuration["Orders:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<MongoRepository<OrderModel>>>();
            return new MongoRepository<OrderModel>(client, settings, logger);
        });
    }
}