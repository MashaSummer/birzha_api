using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductMicroservice.Web.Endpoints.MongoTestEndpoint.ViewModels;

public class ProductViewModel
{
    [BsonId]
    [BsonIgnoreIfNull]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = null!;
    
    [BsonElement("best_ask")]
    public double? BestAsk { get; set; }
    
    [BsonElement("best_bid")]
    public double? BestBid { get; set; }

}