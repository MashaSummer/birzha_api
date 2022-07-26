using MongoDB.Bson.Serialization.Attributes;

namespace ProductMicroservice.Definitions.Mongodb.Models;

public class ProductModel
{
    [BsonId]
    [BsonIgnoreIfNull]
    public string Id { get; set; }
    
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("best_ask")]
    public double BestAsk { get; set; }
    
    [BsonElement("best_bid")]
    public double BestBid { get; set; }
}