using MongoDB.Driver;
using ProductMicroservice.Web.DbWorker;
using ProductMicroservice.Web.Definitions.Base;

namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongodbDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoSettings").Get<MongoDbSettings>();
        services.AddSingleton(mongoSettings);
        services.AddTransient<IMongoClient>(provider => new MongoClient(mongoSettings.ConnectionString));
        services.AddSingleton<IDbWorker, MongoDbWorker>();
    }
}