using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.Mongodb.Models;

public enum OrderTypes
{
    Bid,
    Ask
}


public enum OrderStatus
{
    Validating,
    Validated,
    Active,
    Executing,
    Executed,
    Aborted
}

public class OrderModel : IMongoModel
{
    [BsonId]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("type")]
    public OrderTypes OrderType { get; set; }

    [BsonElement("product_id")] public string ProductId { get; set; } = null!;

    [BsonElement("volume")] public double Volume;

    [BsonElement("price")]
    public double Price;

    [BsonElement("investor_id")] public string InvestorId = null!;
    
    [BsonElement("only_full_execution")]
    [BsonIgnoreIfNull]
    public bool? OnlyFullExecution = null;
    
    [BsonElement("deadline")]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime? Deadline = null;


    [BsonElement("status")]
    public OrderStatus Status { get; set; }

}