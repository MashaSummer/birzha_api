using Calabonga.OperationResults;
using OrdersMicroservice.Domain.IServices;

namespace OrdersMicroservice.Web.Definitions.DepthMarket.Services
{
    public class DepthMarketService : IDepthMarketService
    {
        public async Task<OperationResult<bool>> ProcessOrderAsync(string orderId)
        {
            // Alg:
            // 1.DefineType
            // 

            throw new NotImplementedException();
        }

        
    }
}
