using Calabonga.OperationResults;
using PortfolioMicroService.Domain.EventsBase;
using Confluent.Kafka;
using AutoMapper;
using PortfolioMicroservice.Domain.DbBase;
using PortfolioMicroService.Definitions.Mongodb.Models;

namespace PortfolioMicroService.Definitions.Kafka.Handlers
{
    public class OrdersExecuteHandler : IEventHandler<Null, OrderExecuteEvent>
    {
        private readonly IRepository<UserModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public OrdersExecuteHandler(IRepository<UserModel> repository, IMapper mapper, ILogger<AddProductHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public void Process(Message<Null, OrderExecuteEvent> message)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderExecuteEvent> message)
        {
            var askPortfolio = await _repository.GetByIdAsync(message.Value.AskInvestorId);
            var bidPortfolio = await _repository.GetByIdAsync(message.Value.BidInvestorId);

            
            await _repository.UpdateAsync(EncreaseBid(bidPortfolio.Result, message.Value));
            await _repository.UpdateAsync(DecreaseAsk(askPortfolio.Result, message.Value.ProductId, message.Value.Volume));


            return new OperationResult<bool>() { Result = true };
        }

        private UserModel DecreaseAsk(UserModel askPortfolio, string assetId, int volume)
        {
            int index = askPortfolio.Asset.ToList().FindIndex(x => x.Id == assetId);

            askPortfolio.Asset[index].VolumeFrozen -= volume;
            return askPortfolio;
        } 

        private UserModel EncreaseBid(UserModel bidPortfolio, OrderExecuteEvent message)
        {
            int index = bidPortfolio.Asset.ToList().FindIndex(x => x.Id == message.ProductId);

            if (index == -1)
            {
                bidPortfolio.Asset = bidPortfolio.Asset.Append(_mapper.Map<AssetModel>(message)).ToArray();
                return bidPortfolio;
            }

            bidPortfolio.Asset[index].VolumeActive += message.Volume;
            return bidPortfolio;
        }
    }
}
