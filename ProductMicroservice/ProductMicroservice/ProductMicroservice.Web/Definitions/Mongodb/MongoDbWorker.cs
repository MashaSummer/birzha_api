using MongoDB.Driver;
using ProductMicroservice.Web.DbWorker;

namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongoDbWorker : IDbWorker
{
    private readonly IMongoClient _mongoClient;
    private readonly ILogger<MongoDbWorker> _logger;

    private readonly MongoDbSettings _settings;

    public MongoDbWorker(IMongoClient mongoClient, ILogger<MongoDbWorker> logger)
    {
        _mongoClient = mongoClient;
        _logger = logger;
    }

    public T GetAllRecord<T>() => throw new NotImplementedException();
}