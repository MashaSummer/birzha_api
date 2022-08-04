using Calabonga.OperationResults;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Infrastructure.Mongodb.Context;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace OrdersMicroservice.Infrastructure.Mongodb;

/// <summary>
/// Realization of IDbWorker for easy work with mongodb
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class MongodbWorker<T> : IDbWorker<T>
{
    private readonly ILogger<MongodbWorker<T>> _logger;
    private readonly IMongoDbContext<T> _context;

    public MongodbWorker(ILogger<MongodbWorker<T>> logger, IMongoDbContext<T> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task<IEnumerable<T>> GetAllRecords()
    {
        return Task.FromResult<IEnumerable<T>>(_context);
    }

    public Task<IEnumerable<T>> GetRecordsByFilter(Func<T, bool> predicate)
    {
        var result = _context.Where(predicate);

        return Task.FromResult(result);
    }

    public async Task<OperationResult<bool>> AddNewRecord(T record)
    {
        OperationResult<bool> result = new OperationResult<bool>();
        try
        {
            await _context.GetCollection().InsertOneAsync(record);
            result.Result = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.Result = false;
            result.AddError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> AddNewRecordsRange(IEnumerable<T> records)
    {
        OperationResult<bool> result = new OperationResult<bool>();
        try
        {
            await _context.GetCollection().InsertManyAsync(records);
            result.Result = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.Result = false;
            result.AddError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> UpdateRecords(Func<T, bool> predicate, Action<T> updateFunc)
    {
        OperationResult<bool> result = new OperationResult<bool>();

        long totalUpdated = 0;

        try
        {
            var updatingItems = _context.Where(predicate);

            foreach (var updatingItem in updatingItems)
            {
                var id = typeof(T).GetProperty("Id")?.GetValue(updatingItem);
                var filter = Builders<T>.Filter.Eq("_id", id);

                updateFunc(updatingItem);

                var replacementResult = await _context.GetCollection().ReplaceOneAsync(filter, updatingItem);
                totalUpdated += replacementResult.ModifiedCount;
            }

            _logger.LogInformation($"Updated {totalUpdated} records");
            result.Result = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }


        return result;
    }
    public async Task<OperationResult<bool>> DeleteRecords(Func<T, bool> predicate)
    {
        var result = new OperationResult<bool>();

        long totalDeleted = 0;
        try
        {
            var deletingItems = _context.Where(predicate);

            foreach (var deletingItem in deletingItems)
            {
                var id = typeof(T).GetProperty("Id")?.GetValue(deletingItem);
                var filter = Builders<T>.Filter.Eq("_id", id);

                var deletingResult = await _context.GetCollection().DeleteOneAsync(filter);
                totalDeleted += deletingResult.DeletedCount;
            }

            _logger.LogInformation($"Deleted {totalDeleted} items");
            result.Result = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            result.AddError(e);
        }

        return result;
    }
}