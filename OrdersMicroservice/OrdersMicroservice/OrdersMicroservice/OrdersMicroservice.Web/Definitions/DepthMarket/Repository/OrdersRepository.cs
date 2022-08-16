using MongoDB.Driver;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.DepthMarket.Repository
{
    public class OrdersRepository
    {
        private readonly IMongoCollection<OrderModel> _ordersCollection;

        public OrdersRepository(
            IMongoClient client,
            MongodbSettings settings,
            ILogger<OrdersRepository> logger)
        {
            _ordersCollection = client.GetDatabase(settings.DbName).GetCollection<OrderModel>(settings.CollectionName);
        }
        public async Task<string> CreateNewAsync(OrderModel newOrder)
        {
            await _ordersCollection.InsertOneAsync(newOrder);
            return newOrder.Id;
        }
        public async Task<OrderModel> GetByIdAsync(string id)
        {
            return await _ordersCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
        }
        public async Task<List<OrderModel>> GetAllAsync()
        {
            return await _ordersCollection.Find(_ => true).ToListAsync();
        }
        public async Task UpdateAsync(OrderModel orderToUpdate)
        {
            await _ordersCollection.ReplaceOneAsync(x => x.Id == orderToUpdate.Id, orderToUpdate);
        }
        public async Task DeleteAsync(string id)
        {
            await _ordersCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<int> GetEarnedAsync(string investorId, string productId)
        {

            var collection = await _ordersCollection.Find(x => x.InvestorId.Equals(investorId)
                                                               && x.ProductId.Equals(productId)
                                                               && x.Status == OrderStatus.Executed
                                                               && x.OrderType == OrderTypes.Ask).ToListAsync();
            if (collection == null)
                return 0;

            return collection.Sum(x => x.Price);
        }

        public async Task<int> GetSpentAsync(string investorId, string productId)
        {
            var collection = await _ordersCollection.Find(x => x.InvestorId.Equals(investorId)
                                                               && x.ProductId.Equals(productId)
                                                               && x.Status == OrderStatus.Executed
                                                               && x.OrderType == OrderTypes.Bid).ToListAsync();
            return collection?.Sum(x => x.Price) ?? 0;
        }
    }
}
