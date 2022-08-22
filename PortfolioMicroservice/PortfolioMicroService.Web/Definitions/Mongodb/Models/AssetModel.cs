using PortfolioMicroService.Infrastructure.Mongodb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PortfolioMicroService.Definitions.Mongodb.Models;

public class AssetModel : IMongoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("start_price")]
    public double StartPrice { get; set; }

    [BsonElement("volume_active")] 
    public int VolumeActive { get; set; }

    [BsonElement("volume_frozen")] 
    public int VolumeFrozen { get; set; }
}