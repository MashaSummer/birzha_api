using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

public class OrderModel
{
    [BsonId]
    [BsonIgnoreIfNull]
    public ObjectId Id { get; set; }
    
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
    public BsonDateTime? Deadline = null;


    [BsonElement("status")]
    public OrderStatus Status { get; set; }

}