using Calabonga.OperationResults;

namespace PortfolioMicroService.Domain.DbBase;

public interface IRepository<T>
{

    Task<OperationResult<T>> GetUserByIdAsync(string id);

    Task<OperationResult<T>> TryFrozeByIdAsync(string id, string asset_id, int volume);

    Task<OperationResult<T>> TryUnFrozeByIdAsync(string id, string asset_id, int volume);

    Task<OperationResult<long>> UpdateAsync(string id, string asset_id, int volume);

    Task<OperationResult<T>> AddUserAsync(string id);

    Task<OperationResult<T>> AddAssetAsync(string id, string new_asset_id, int volume);
}