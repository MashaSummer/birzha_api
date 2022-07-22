namespace ProductMicroservice.Web.DbWorker;

public interface IDbWorker
{
    Task<IEnumerable<T>> GetAllRecord<T>();
}