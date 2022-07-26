namespace BalanceMicroservice.Web.MongoService.ViewModels
{
    public class BalanceStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string BalanceCollectionName { get; set; } = null!;
    }
}
