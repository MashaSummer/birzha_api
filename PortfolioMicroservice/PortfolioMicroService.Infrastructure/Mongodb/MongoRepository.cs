using Calabonga.OperationResults;
using LightMicroserviceModule.Domain.DbBase;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LightMicroserviceModule.Infrastructure.Mongodb;

public class MongoRepository<T> : IRepository<T> where T : IMongoModel
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILogger<MongoRepository<T>> _logger;

    public MongoRepository(IMongoClient client, MongodbSettings settings, ILogger<MongoRepository<T>> logger)
    {
        _logger = logger;
        _collection = client.GetDatabase(settings.DbName).GetCollection<T>(settings.CollectionName);
    }

    public async Task<OperationResult<IEnumerable<T>>> GetAllAsync()
    {
        var result = new OperationResult<IEnumerable<T>>();

        try
        {
            result.Result = await _collection.Find(new BsonDocument()).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<T>> GetByIdAsync(string id)
    {
        var result = new OperationResult<T>();

        try
        {
            var entity = await _collection.Find(_ => _.Id == id).FirstOrDefaultAsync();
            if (entity == null)
            {
                result.AddError(new Exception($"No entities with this id: {id}"));
                return result;
            }

            result.Result = entity;

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<T>> Get(Func<T, bool> predicate)
    {
        var result = new OperationResult<T>();

        try
        {
            var tmp = _collection.AsQueryable().Where(predicate).FirstOrDefault();
            if (tmp == null)
            {
                var errorString = "Can't find an object";
                _logger.LogError(errorString);
                result.AddError(new Exception(errorString));
            }

            result.Result = tmp!;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }


    public async Task<OperationResult<long>> UpdateAsync(T entity)
    {
        var result = new OperationResult<long>();

        try
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.Id));
            var updateResult = await _collection.ReplaceOneAsync(filter, entity);

            result.Result = updateResult.ModifiedCount;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<long>> UpdateAsync(params T[] entities)
    {
        var result = new OperationResult<long>();

        try
        {
            var ids = entities.Select(_ => _.Id).ToList();

            for (int i = 0; i < entities.Length; i++)
            {
                var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(ids[i]));
                var updatingResult = await _collection.ReplaceOneAsync(filter, entities[i]);
                result.Result += updatingResult.ModifiedCount;
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<string>> AddAsync(T entity)
    {
        var result = new OperationResult<string>();

        try
        {
            await _collection.InsertOneAsync(entity);

            result.Result = entity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<string[]>> AddAsync(T[] entities)
    {
        var result = new OperationResult<string[]>();

        try
        {
            await _collection.InsertManyAsync(entities);

            result.Result = entities.Select(_ => _.Id).ToArray();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<bool>> DeleteAsync(T entity)
    {
        var result = new OperationResult<bool>();

        try
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(entity.Id));
            await _collection.DeleteOneAsync(filter);
            result.Result = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<long>> DeleteAsync(params T[] entities)
    {
        var result = new OperationResult<long>();

        try
        {
            var filter = Builders<T>.Filter.In("_id", entities.Select(_ => ObjectId.Parse(_.Id)));
            var deletingResult = await _collection.DeleteManyAsync(filter);
            result.Result = deletingResult.DeletedCount;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }

    public async Task<OperationResult<long>> DeleteAsync(Func<T, bool> predicate)
    {
        var result = new OperationResult<long>();

        try
        {
            var ids = _collection.AsQueryable().Where(predicate);
            result = await DeleteAsync(ids.ToArray());
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }
}