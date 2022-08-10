using Calabonga.OperationResults;
using PortfolioMicroService.Domain.DbBase;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
namespace PortfolioMicroService.Infrastructure.Mongodb;

public class MongoRepository<T> : IRepository<T> where T : IMongoModel
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoClient client, MongodbSettings settings, ILogger<MongoRepository<T>> logger)
    {
        _logger = logger;
        _collection = client.GetDatabase(settings.DbName).GetCollection<T>(settings.CollectionName);
    }

    public Task<OperationResult<T>> AddAssetAsync(string id, string new_asset_id, int volume)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<T>> AddUserAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<T>> GetUserByIdAsync(string id) =>
        Task.FromResult(OperationResult.CreateResult(_collection.Find(x => x.Id == id).First()));
    

    public Task<OperationResult<T>> TryFrozeByIdAsync(string id, string asset_id, int volume)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<T>> TryUnFrozeByIdAsync(string id, string asset_id, int volume)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<long>> UpdateAsync(string id, string asset_id, int volume)
    {
        throw new NotImplementedException();
    }
}