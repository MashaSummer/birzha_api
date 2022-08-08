using MongoDB.Driver;
using ProductMicroservice.Definitions.Base;
using ProductMicroservice.Domain.DbBase;
using ProductMicroservice.Infrastructure.Mongodb;
using ProductMicroservice.Mongodb.Models;

namespace ProductMicroservice.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IRepository<ProductModel>>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Products:Collection"],
                DbName = configuration["Products:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<MongoRepository<ProductModel>>>();
            return new MongoRepository<ProductModel>(client, settings, logger);
        });
    }
}