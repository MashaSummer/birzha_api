using MongoDB.Bson.Serialization.Attributes;
using NewPortfolioMicroservice.Infrastructure.Mongodb;

namespace NewPortfolioMicroservice.Definitions.Mongodb.Models;

public class AssetModel : IMongoModel
{
    [BsonElement("product_id")]
    public string Id { get; set; }

    [BsonElement("start_price")]
    public int StartPrice { get; set; }

    [BsonElement("volume_active")] 
    public int VolumeActive { get; set; }

    [BsonElement("volume_frozen")] 
    public int VolumeFrozen { get; set; }
}
