using AutoMapper;
using PortfolioMicroService.Definitions.Base;
using PortfolioMicroService.Definitions.Mongodb.Models;
using PortfolioMicroService.Infrastructure.Mongodb;
using MongoDB.Driver;
using PortfolioMicroservice.Domain.DbBase;
using PortfolioMicroserviceModule.Infrastructure.Mongodb;

namespace PortfolioMicroService.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IRepository<UserModel>>(provider => 
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Portfolio:Collection"],
                DbName = configuration["Portfolio:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<MongoRepository<UserModel>>>();
            return new MongoRepository<UserModel>(client, settings, logger);
        });
    }
}