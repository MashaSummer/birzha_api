using OrdersMicroservice.Definitions.Base;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;
using MongoDB.Driver;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Definitions.DepthMarket.Repository;

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
        services.AddSingleton<AskMarketRepository>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Asks:Collection"],
                DbName = configuration["Asks:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<AskMarketRepository>>();
            return new AskMarketRepository(client, settings, logger);
        });
        services.AddSingleton<BidMarketRepository>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Bids:Collection"],
                DbName = configuration["Bids:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<BidMarketRepository>>();
            return new BidMarketRepository(client, settings, logger);
        });
        services.AddSingleton<OrdersRepository>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Orders:Collection"],
                DbName = configuration["Orders:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<OrdersRepository>>();
            return new OrdersRepository(client, settings, logger);
        });
    }
}