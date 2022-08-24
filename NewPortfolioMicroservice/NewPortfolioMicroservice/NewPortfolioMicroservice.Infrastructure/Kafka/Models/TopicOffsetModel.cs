using Confluent.Kafka;
using NewPortfolioMicroservice.Infrastructure.Mongodb;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NewPortfolioMicroservice.Infrastructure.Kafka.Models;

public class TopicOffsetModel : IMongoModel
{
    [BsonId]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("partition")] public int Partition { get; set; }

    [BsonElement("offset")] public long Offset { get; set; }


    [BsonElement("topic")] public string Topic { get; set; } = null!;


    public TopicPartitionOffset BuildTopicPartitionOffset() =>
        new TopicPartitionOffset(Topic, BuildPartition(), BuildOffset());

    public Partition BuildPartition() => new Partition(Partition);

    public Offset BuildOffset() => new Offset(Offset);
}