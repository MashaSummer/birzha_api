using Calabonga.OperationResults;

namespace ProductMicroservice.Domain.DbBase;

/// <summary>
/// Abstraction for work with database
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public interface IDbWorker<T>
{
    Task<IEnumerable<T>> GetAllRecords();

    Task<IEnumerable<T>> GetRecordsByFilter(Func<T, bool> predicate);

    Task<OperationResult<bool>> AddNewRecord(T record);

    Task<OperationResult<bool>> AddNewRecordsRange(IEnumerable<T> records);
}