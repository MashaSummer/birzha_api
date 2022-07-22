namespace ProductMicroservice.Web.DbWorker;

public interface IDbWorker
{
    T GetAllRecord<T>();
}