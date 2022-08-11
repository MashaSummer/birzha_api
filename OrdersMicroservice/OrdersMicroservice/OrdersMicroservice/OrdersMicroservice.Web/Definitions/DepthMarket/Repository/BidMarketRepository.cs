using MongoDB.Driver;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.DepthMarket.Repository
{
    public class BidMarketRepository : MongoRepository<MarketModel>
    {
        public BidMarketRepository(IMongoClient client, MongodbSettings settings, ILogger<MongoRepository<MarketModel>> logger) : base(client, settings, logger)
        {
        }
    }
}
