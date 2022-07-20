using BalanceMicroservice.Web.Endpoints.BalanceEndpoints.ViewModels;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints
{
    public class MongoController
    {
        private readonly IMongoCollection<BalanceViewModel> _balancesCollection;

        public MongoController(IOptions<BalanceStoreDatabaseSettings> balanceStoreDatabaseSettings)
        {
            var mongoCLient = new MongoClient(
                balanceStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoCLient.GetDatabase(
                balanceStoreDatabaseSettings.Value.DatabaseName);
            _balancesCollection = mongoDatabase.GetCollection<BalanceViewModel>(
                balanceStoreDatabaseSettings.Value.BalanceCollectionName);
        }

        public async void SetValue()
        {
            await _balancesCollection.InsertOneAsync(new BalanceViewModel
            {
                Id = new Guid("FF0186C5-C3A5-4668-9641-83FDFC111571"),
                Balance = 150
            });
        }

        public async Task<List<BalanceViewModel>> GetAsync() =>
            await _balancesCollection.Find(_ => true).ToListAsync();
        public async Task<BalanceViewModel> GetAsync(Guid id) =>
            await _balancesCollection.Find(x => x.Id == id).FirstAsync();
        public async Task CreateAsync(BalanceViewModel balance) =>
            await _balancesCollection.InsertOneAsync(balance);
        public async Task UpdateAsync(BalanceViewModel updatedBalance) =>
            await _balancesCollection.ReplaceOneAsync(x => x.Id == updatedBalance.Id, updatedBalance);
    }
}
