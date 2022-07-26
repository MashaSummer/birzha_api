using AutoMapper;
using ProductMicroservice.Definitions.Base;
using ProductMicroservice.Definitions.Mongodb.Models;
using ProductMicroservice.Infrastructure.Mongodb;
using ProductMicroservice.Infrastructure.Mongodb.Context;
using MongoDB.Driver;
using ProductMicroservice.Domain.DbBase;

namespace ProductMicroservice.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IMongoDbContext<ProductModel>>(provider => new MongodbContext<ProductModel>(
            new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Products:Collection"],
                DbName = configuration["Products:Database"]
            }, provider.GetRequiredService<IMongoClient>()));

        services.AddSingleton<IDbWorker<ProductModel>, MongodbWorker<ProductModel>>();
    }
}