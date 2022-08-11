using Calabonga.OperationResults;

namespace LightMicroserviceModule.Domain.DbBase;

public interface IRepository<T>
{
    Task<OperationResult<IEnumerable<T>>> GetAllAsync();

    Task<OperationResult<T>> GetByIdAsync(string id);

    Task<OperationResult<T>> Get(Func<T, bool> predicate);

    Task<OperationResult<long>> UpdateAsync(T entity);

    Task<OperationResult<long>> UpdateAsync(params T[] entities);

    Task<OperationResult<string>> AddAsync(T entity);

    Task<OperationResult<string[]>> AddAsync(params T[] entities);

    Task<OperationResult<bool>> DeleteAsync(T entity);

    Task<OperationResult<long>> DeleteAsync(params T[] entities);

    Task<OperationResult<long>> DeleteAsync(Func<T, bool> predicate);
}