using OrdersMicroservice.Domain.DbBase;
using MongoDB.Driver;

namespace OrdersMicroservice.Infrastructure.Mongodb.Context;

/// <summary>
/// Custom context for mongo database
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public interface IMongoDbContext<T> : IDbContext<T>
{
    IMongoCollection<T> GetCollection();
}