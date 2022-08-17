using MongoDB.Driver;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.DepthMarket.Repository
{
    public class AskMarketRepository
    {
        private readonly IMongoCollection<MarketModel> _asksCollection;
        public AskMarketRepository(
            IMongoClient client, 
            MongodbSettings settings,
            ILogger<AskMarketRepository> logger) 
        {
            _asksCollection = client.GetDatabase(settings.DbName).GetCollection<MarketModel>(settings.CollectionName);
            if (!_asksCollection.Indexes.List().Any())
            {
                var indexKeysDefine = Builders<MarketModel>.IndexKeys
                    .Ascending(x => x.ProductId)
                    .Ascending(x => x.Price)
                    .Ascending(x => x.SubmissionTime);
                _asksCollection.Indexes.CreateOne(new CreateIndexModel<MarketModel>(indexKeysDefine));
            }
        }

        public async Task<string> CreateNewAsync(MarketModel ask)
        {
            await _asksCollection.InsertOneAsync(ask);
            return ask.Id;
        }
        public async Task<MarketModel> GetByIdAsync(string id)
        {
            return await _asksCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<MarketModel>> GetAllAsync()
        {
            return await _asksCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(MarketModel askToUpdate)
        {
            await _asksCollection.ReplaceOneAsync(x => x.Id == askToUpdate.Id, askToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _asksCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<MarketModel>> GetRelevantAsksAsync(OrderModel model)
        {
            // по id товара
            // price  меньше или равно
            // дата самые старые - самые первые
            return await _asksCollection.Find(m => m.ProductId == model.ProductId && m.Price <= model.Price)
                .SortByDescending(m => m.Price).SortBy(m => m.SubmissionTime).ToListAsync();
        }

        public async Task DeleteByOrderIdAsync(string orderId)
        {
            await _asksCollection.DeleteOneAsync(x => x.OrderId == orderId);
        }
        public async Task<int> BestAskByProductIdAsync(string productId)
        {
            var bestAsk = await _asksCollection.Find(m => m.ProductId == productId)
                .SortByDescending(m => m.Price).SortBy(m => m.SubmissionTime).FirstOrDefaultAsync();
            return bestAsk?.Price ?? -1;
        }

    }
}
