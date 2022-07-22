using MongoDB.Driver;
using ProductMicroservice.Web.DbWorker;
using ProductMicroservice.Web.Definitions.Base;

namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongodbDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMongoClient>(provider => new MongoClient(configuration.GetConnectionString("mongo")));
        services.AddSingleton<IDbWorker, MongoDbWorker>();
    }
}