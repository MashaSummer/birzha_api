using Calabonga.OperationResults;

namespace OrdersMicroservice.Domain.IServices
{
    public interface IDepthMarketService
    {
        Task<OperationResult<bool>> ProcessOrderAsync(string orderId);
    }
}