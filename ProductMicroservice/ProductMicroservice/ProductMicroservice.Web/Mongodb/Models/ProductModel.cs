using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProductMicroservice.Infrastructure.Mongodb;

namespace ProductMicroservice.Mongodb.Models;

public class ProductModel : IMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonIgnoreIfNull]
    public string Id { get; set; }

    [BsonElement("name")] public string Name { get; set; } = null!;

    [BsonElement("investor_id")] public string InvestorId { get; set; } = null!;
}