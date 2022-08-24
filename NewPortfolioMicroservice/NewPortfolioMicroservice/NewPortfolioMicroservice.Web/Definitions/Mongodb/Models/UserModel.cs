using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NewPortfolioMicroservice.Infrastructure.Mongodb;

namespace NewPortfolioMicroservice.Definitions.Mongodb.Models;

public class UserModel : IMongoModel
{
    [BsonId]
    public string Id { get; set; }

    [BsonElement("Assets")]
    public IList<AssetModel> Assets { get; set; } = new List<AssetModel>();
}
