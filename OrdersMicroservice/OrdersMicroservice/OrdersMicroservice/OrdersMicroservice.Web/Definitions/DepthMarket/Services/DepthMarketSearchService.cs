using OrdersMicroservice.Definitions.DepthMarket.Dto;
using OrdersMicroservice.Definitions.DepthMarket.Repository;
using OrdersMicroservice.Definitions.Mongodb.Models;

namespace OrdersMicroservice.Definitions.DepthMarket.Services
{
    public class DepthMarketSearchService
    {
        private readonly AskMarketRepository _askMarketRepository;
        private readonly OrdersRepository _ordersRepository;
        private readonly BidMarketRepository _bidMarketRepository;
        public DepthMarketSearchService(
            AskMarketRepository askMarketRepository,
            OrdersRepository ordersRepository,
            BidMarketRepository bidMarketRepository
            )
        {
            _askMarketRepository = askMarketRepository;
            _ordersRepository = ordersRepository;
            _bidMarketRepository = bidMarketRepository;
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
        public async Task<List<UserProductInfoDto>> GetProductsInfoAsync (string investorId, List<string> productsId)
        {
            var dtosList = new List<UserProductInfoDto>();
            foreach(var productId in productsId)
            {
                var productInfoDto = new UserProductInfoDto();
                productInfoDto.Earned = await _ordersRepository.GetEarnedAsync(investorId, productId);

                productInfoDto.Spent = await _ordersRepository.GetSpentAsync(investorId, productId);
                productInfoDto.ProductId = productId;

                productInfoDto.BestAsk = await GetBestAskAsync(productId);
                productInfoDto.BestBid = await GetBestBidAsync(productId);

                dtosList.Add(productInfoDto);

            }
            return dtosList;
        }

        public async Task<int> GetBestBidAsync(string productId) => await _bidMarketRepository.BestBidByProductIdAsync(productId);
        
        public async Task<int> GetBestAskAsync(string productId) => await _askMarketRepository.BestAskByProductIdAsync(productId);


    }
}
