using MongoDB.Driver;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.DepthMarket.Repository
{
    public class BidMarketRepository
    {
        private readonly IMongoCollection<MarketModel> _bidsCollection;

        public BidMarketRepository(
            IMongoClient client,
            MongodbSettings settings,
            ILogger<BidMarketRepository> logger)
        {
            _bidsCollection = client.GetDatabase(settings.DbName).GetCollection<MarketModel>(settings.CollectionName);
            if (!_bidsCollection.Indexes.List().Any())
            {
                var indexKeysDefine = Builders<MarketModel>.IndexKeys
                    .Ascending(x => x.ProductId)
                    .Ascending(x => x.Price)
                    .Ascending(x => x.SubmissionTime);
                _bidsCollection.Indexes.CreateOne(new CreateIndexModel<MarketModel>(indexKeysDefine));
            }
        }

        public async Task<string> CreateNewAsync(MarketModel bid)
        {
            await _bidsCollection.InsertOneAsync(bid);
            return bid.Id;
        }
        public async Task<MarketModel> GetByIdAsync(string id)
        {
            return await _bidsCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<MarketModel>> GetAllAsync()
        {
            return await _bidsCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(MarketModel bidToUpdate)
        {
            await _bidsCollection.ReplaceOneAsync(x => x.Id == bidToUpdate.Id, bidToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _bidsCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<MarketModel>> GetRelevantBidsAsync(OrderModel model)
        {
            // по id товара
            // price больше или равно
            // дата самые старые - самые первые
            return await _bidsCollection.Find(m => m.ProductId == model.ProductId && m.Price >= model.Price)
                .SortBy(m => m.Price).SortBy(m => m.SubmissionTime).ToListAsync();
        }

        public async Task DeleteByOrderIdAsync(string orderId)
        {
            await _bidsCollection.DeleteOneAsync(x => x.OrderId == orderId);
        }

        public async Task<int> BestBidByProductIdAsync(string productId)
        {
            var bestBid = await _bidsCollection.Find(m => m.ProductId == productId)
                .SortBy(m => m.Price).SortBy(m => m.SubmissionTime).FirstOrDefaultAsync();
            return bestBid?.Price ?? -1;
        }
    }
}
