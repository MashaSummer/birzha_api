using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrdersMicroservice.Infrastructure.Mongodb;

public interface IMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
}