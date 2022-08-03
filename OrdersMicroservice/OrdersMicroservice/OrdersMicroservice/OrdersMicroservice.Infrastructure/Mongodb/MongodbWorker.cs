using Calabonga.OperationResults;
using OrdersMicroservice.Domain.DbBase;
using OrdersMicroservice.Infrastructure.Mongodb.Context;
using Microsoft.Extensions.Logging;

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
}