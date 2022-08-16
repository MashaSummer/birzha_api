using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrdersMicroservice.Infrastructure.Mongodb;

namespace OrdersMicroservice.Definitions.Mongodb.Models
{
    public class MarketModel : IMongoModel
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("product_id")]
        public string ProductId { get; set; } = null!;

        [BsonElement("order_id")]
        public string OrderId { get; set; } = null!;

        [BsonElement("volume")]
        public int Volume { get; set; }

        [BsonElement("price")]
        public int Price { get; set; }

        [BsonElement("only_full_execution")]
        public bool OnlyFullExecution { get; set; }

        [BsonElement("submission_time")]
        public DateTime SubmissionTime { get; set; }
    }
}
