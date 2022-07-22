using MongoDB.Bson;
using MongoDB.Driver;
using ProductMicroservice.Web.DbWorker;

namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongoDbWorker : IDbWorker
{
    private readonly IMongoClient _mongoClient;
    private readonly ILogger<MongoDbWorker> _logger;

    private readonly MongoDbSettings _settings;

    public MongoDbWorker(IMongoClient mongoClient, ILogger<MongoDbWorker> logger, MongoDbSettings settings)
    {
        _mongoClient = mongoClient;
        _logger = logger;
        _settings = settings;
    }

    public async Task<IEnumerable<T>> GetAllRecord<T>()
    {
        _logger.LogInformation($"Getting all data from {_settings.DbName}:{_settings.CollectionName}");
        
        var database = _mongoClient.GetDatabase(_settings.DbName);
        var collection = database.GetCollection<T>(_settings.CollectionName);

        var filter = new BsonDocument();
        var records = await collection.Find(filter).ToListAsync();   // Warning! Find load all data in memory!!!

        return records;
    }
}