using ProductMicroservice.Web.DbWorker;

namespace ProductMicroservice.Web.Definitions.Mongodb;

public class MongoDbWorker : IDbWorker
{
    public T GetAllRecord<T>() => throw new NotImplementedException();
}