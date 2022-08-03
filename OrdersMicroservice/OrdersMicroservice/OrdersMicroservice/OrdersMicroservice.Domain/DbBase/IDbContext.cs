namespace OrdersMicroservice.Domain.DbBase;

/// <summary>
/// Layer between database and db worker
/// </summary>
/// <typeparam name="T">Model type</typeparam>
public interface IDbContext<out T> : IEnumerable<T>
{
}