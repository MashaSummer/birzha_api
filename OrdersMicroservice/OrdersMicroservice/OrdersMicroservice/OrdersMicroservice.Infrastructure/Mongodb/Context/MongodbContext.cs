using System.Collections;
using MongoDB.Driver;

namespace OrdersMicroservice.Infrastructure.Mongodb.Context;

/// <summary>
/// Realization of IMongoDbContext for work with mongodb collection
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public class MongodbContext<T> : IMongoDbContext<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongodbContext(MongodbSettings settings, IMongoClient client) =>
        _collection = client.GetDatabase(settings.DbName).GetCollection<T>(settings.CollectionName);

    public IMongoCollection<T> GetCollection() => _collection;

    public IEnumerator<T> GetEnumerator() => _collection.AsQueryable().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}