using BalanceMicroservice.Web.Endpoints.BalanceEndpoints.ViewModels;
using BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BalanceMicroservice.Web.Endpoints.BalanceEndpoints
{
    public class BalanceService
    {
        private readonly IMongoCollection<BalanceViewModel> _balancesCollection;

        public BalanceService(IOptions<BalanceStoreDatabaseSettings> balanceStoreDatabaseSettings)
        {
            var mongoCLient = new MongoClient(
                balanceStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoCLient.GetDatabase(
                balanceStoreDatabaseSettings.Value.DatabaseName);
            _balancesCollection = mongoDatabase.GetCollection<BalanceViewModel>(
                balanceStoreDatabaseSettings.Value.BalanceCollectionName);
        }

        public async Task<List<BalanceViewModel>> GetAsync() =>
            await _balancesCollection.Find(_ => true).ToListAsync();
        public async Task<BalanceViewModel?> GetAsync(Guid id) =>
            await _balancesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        public async Task CreateAsync(BalanceViewModel balance) =>
            await _balancesCollection.InsertOneAsync(balance);
        public async Task UpdateAsync(Guid id, BalanceViewModel updatedBalance) =>
            await _balancesCollection.ReplaceOneAsync(x => x.Id == id, updatedBalance);
    }
}
