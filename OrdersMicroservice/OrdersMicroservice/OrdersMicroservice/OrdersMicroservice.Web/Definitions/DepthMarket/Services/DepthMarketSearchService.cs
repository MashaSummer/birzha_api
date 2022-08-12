using OrdersMicroservice.Definitions.DepthMarket.Repository;
using OrdersMicroservice.Definitions.Mongodb.Models;

namespace OrdersMicroservice.Definitions.DepthMarket.Services
{
    public class DepthMarketSearchService
    {
        private readonly AskMarketRepository _askMarketRepository;
        public DepthMarketSearchService(AskMarketRepository askMarketRepository)
        {
            _askMarketRepository = askMarketRepository;
        }
        public MarketModel FullExecSearch(OrderModel model, List<MarketModel> relevantMarketList)
        {
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if (model.Volume == listItem.Volume)
                        return listItem;
                    continue;
                }
                if (listItem.Volume >= model.Volume)
                    return listItem;
            }
            return null;
        }
        public List<MarketModel> PartialExecSearch(OrderModel model, List<MarketModel> relevantMarketList)
        {
            var existingVolume = model.Volume;
            var candidatesList = new List<MarketModel>();
            foreach (var listItem in relevantMarketList)
            {
                if (listItem.OnlyFullExecution)
                {
                    if (existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                }
                else
                {
                    if (existingVolume >= listItem.Volume)
                    {
                        existingVolume -= listItem.Volume;
                        candidatesList.Add(listItem);
                    }
                    else
                    {
                        candidatesList.Add(listItem);
                        break;
                    }
                }
            }
            return candidatesList;
        }
        public async Task GetDepthMarketAsync(string productId)
        {

        }
        public async Task<bool> HasAskAsync(string productId) 
        {
            return (await _askMarketRepository.GetAllAsync())
                .Any(x => x.ProductId == productId);
        }
    }
}
