using AutoMapper;
using NewPortfolioMicroservice.Definitions.Base;
using NewPortfolioMicroservice.Definitions.Mongodb.Models;
using NewPortfolioMicroservice.Definitions.Mongodb.ViewModels;
using NewPortfolioMicroservice.Domain.DbBase;
using NewPortfolioMicroservice.Infrastructure.Mongodb;
using MongoDB.Driver;

namespace NewPortfolioMicroservice.Definitions.Mongodb;

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