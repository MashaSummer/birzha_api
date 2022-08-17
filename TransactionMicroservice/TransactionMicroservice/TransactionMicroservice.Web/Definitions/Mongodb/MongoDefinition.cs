using AutoMapper;
using TransactionMicroservice.Definitions.Base;
using TransactionMicroservice.Definitions.Mongodb.Models;
using TransactionMicroservice.Domain.DbBase;
using TransactionMicroservice.Infrastructure.Mongodb;
using MongoDB.Driver;

namespace TransactionMicroservice.Definitions.Mongodb;

public class MongoDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mongo");

        services.AddTransient<IMongoClient>(provider => new MongoClient(connectionString));

        services.AddSingleton<IRepository<TransactionModel>>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var settings = new MongodbSettings()
            {
                ConnectionString = connectionString,
                CollectionName = configuration["Transactions:Collection"],
                DbName = configuration["Transactions:Database"]
            };
            var logger = provider.GetRequiredService<ILogger<MongoRepository<TransactionModel>>>();
            return new MongoRepository<TransactionModel>(client, settings, logger);
        });
    }
}