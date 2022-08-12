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

    [BsonElement("volume")]
    public int Volume { get; set; }

    [BsonElement("price")]
    public int Price { get; set; }

    [BsonElement("investor_id")] public string InvestorId { get; set; } = null!;

    [BsonElement("only_full_execution")]
    public bool OnlyFullExecution { get; set; }

    [BsonElement("deadline")]

    public DateTime Deadline { get; set; }

    [BsonElement("submission_time")]
    public DateTime SubmissionTime { get; set; }


    [BsonElement("status")]
    public OrderStatus Status { get; set; }

}