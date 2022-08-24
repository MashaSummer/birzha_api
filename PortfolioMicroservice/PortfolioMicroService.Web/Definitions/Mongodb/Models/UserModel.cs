using PortfolioMicroService.Infrastructure.Mongodb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PortfolioMicroService.Definitions.Mongodb.Models;

public class UserModel : IMongoModel
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("Assets")]
    public AssetModel[]? Asset { get; set; } = null;
}
