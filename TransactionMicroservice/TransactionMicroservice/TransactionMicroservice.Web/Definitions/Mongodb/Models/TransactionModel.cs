using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TransactionMicroservice.Infrastructure.Mongodb;

namespace TransactionMicroservice.Definitions.Mongodb.Models;

public class TransactionModel : IMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    public string Id { get; set; }

    [BsonElement("bid_id")] public string BidId { get; set; } = null!;

    [BsonElement("ask_id")] public string AskId { get; set; } = null!;
    
    [BsonElement("created_time")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime CreatedTime { get; set; }
}