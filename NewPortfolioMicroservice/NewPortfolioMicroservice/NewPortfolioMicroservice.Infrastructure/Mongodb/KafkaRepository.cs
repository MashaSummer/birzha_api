using Calabonga.OperationResults;
using NewPortfolioMicroservice.Infrastructure.Kafka.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace NewPortfolioMicroservice.Infrastructure.Mongodb;

public class KafkaRepository : MongoRepository<TopicOffsetModel>
{
    public KafkaRepository(IMongoClient client, MongodbSettings settings,
        ILogger<MongoRepository<TopicOffsetModel>> logger) : base(
        client, settings, logger)
    {
    }

    public async Task<OperationResult<bool>> CommitMessage(TopicOffsetModel model)
    {
        var result = OperationResult.CreateResult<bool>();
        var offsetFromDbResult = await Get(tom => tom.Partition == model.Partition);

        if (offsetFromDbResult.Ok)
        {
            model.Id = offsetFromDbResult.Result.Id;
            var updateResult = await UpdateAsync(model);
            if (!updateResult.Ok)
                result.AddError(updateResult.Error);
        }
        else
        {
            var addingResult = await AddAsync(model);
            if (!addingResult.Ok)
                result.AddError(addingResult.Error);
        }

        return result;
    }
}