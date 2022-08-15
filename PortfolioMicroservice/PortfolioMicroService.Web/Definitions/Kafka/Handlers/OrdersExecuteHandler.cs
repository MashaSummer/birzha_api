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

            int index = 0;

            askPortfolio.Result.Asset!.Where(x => x.Id == message.Value.ProductId).First().VolumeFrozen -= message.Value.Volume;
            var bidAsset = bidPortfolio.Result.Asset!.Where((x, _index) => 
            {
                index = _index;
                return x.Id == message.Value.ProductId;
            } );

            if (bidAsset.Count() == 0)
            {
                bidPortfolio.Result.Asset!.Append(_mapper.Map<AssetModel>(message));
            }
            else
            {
                bidPortfolio.Result.Asset[index].VolumeActive += message.Value.Volume;
            }

            await _repository.UpdateAsync(bidPortfolio.Result);
            await _repository.UpdateAsync(askPortfolio.Result);

            return new OperationResult<bool>() { Result = true };
        }
    }
}
