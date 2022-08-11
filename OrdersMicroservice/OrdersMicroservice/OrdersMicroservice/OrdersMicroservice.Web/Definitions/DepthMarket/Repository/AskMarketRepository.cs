using MongoDB.Driver;
using OrdersMicroservice.Definitions.Mongodb.Models;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.DepthMarket.Repository
{
    public class AskMarketRepository : MongoRepository<MarketModel>
    {
        public AskMarketRepository(
            IMongoClient client, 
            MongodbSettings settings,
            ILogger<MongoRepository<MarketModel>> logger) 
            : base(client, settings, logger)
        {
        }
    }
}
