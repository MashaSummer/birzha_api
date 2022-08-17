namespace TransactionMicroservice.Infrastructure.Mongodb;

public class MongodbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DbName { get; set; } = null!;

    public string CollectionName { get; set; } = null!;
}