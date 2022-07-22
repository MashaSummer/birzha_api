namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongoDbSettings
{
    public string Host { get; set; } = null!;
    public string Port { get; set; } = null!;
    
    public string ConnectionString => $"mongodb://{Host}:{Port}";

    public string DbName { get; set; } = null!;

    public string CollectionName { get; set; } = null!;
}