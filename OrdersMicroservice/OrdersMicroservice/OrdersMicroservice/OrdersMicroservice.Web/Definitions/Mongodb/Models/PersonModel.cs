using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrdersMicroservice.Definitions.Mongodb.Models;

public class PersonModel
{
    [BsonId] [BsonIgnoreIfNull] public ObjectId Id { get; set; }

    [BsonElement("first_name")] public string FirstName { get; set; } = null!;

    [BsonElement("last_name")] public string LastName { get; set; } = null!;

    [BsonElement("skills")]
    [BsonIgnoreIfNull]
    public string[] Skills { get; set; }
}