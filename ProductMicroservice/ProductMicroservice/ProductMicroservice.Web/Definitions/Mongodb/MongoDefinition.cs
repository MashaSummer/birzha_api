using AutoMapper;
using ProductMicroservice.Definitions.Base;
using ProductMicroservice.Definitions.Mongodb.Models;
using ProductMicroservice.Definitions.Mongodb.ViewModels;
using ProductMicroservice.Infrastructure.Mongodb;
using ProductMicroservice.Infrastructure.Mongodb.Context;
using MongoDB.Driver;

namespace ProductMicroservice.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        /*var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IMongoDbContext<PersonModel>>(provider => new MongodbContext<PersonModel>(
            new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Person:Collection"],
                DbName = configuration["Person:Database"]
            }, provider.GetRequiredService<IMongoClient>()));

        services.AddSingleton<MongodbWorker<PersonModel>>();*/
    }
}