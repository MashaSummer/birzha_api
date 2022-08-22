using Calabonga.OperationResults;
using PortfolioMicroService.Domain.EventsBase;
using Confluent.Kafka;
using AutoMapper;
using PortfolioMicroservice.Domain.DbBase;
using PortfolioMicroService.Definitions.Mongodb.Models;

namespace PortfolioMicroService.Definitions.Kafka.Handlers
{
    public class AddProductHandler : IEventHandler<Null, ProductAddEvent>
    {
        private readonly IRepository<UserModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddProductHandler(IRepository<UserModel> repository, IMapper mapper, ILogger<AddProductHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public void Process(Message<Null, ProductAddEvent> message)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> ProcessAsync(Message<Null, ProductAddEvent> message)
        {
            var portfolio = await _repository.GetByIdAsync(message.Value.InvestorId);

            if (!portfolio.Ok) 
            {
                _logger.LogError("Investor id ({0}) not found", message.Value.InvestorId);
                return new OperationResult<bool>() { Result = false }; 
            }

            portfolio.Result.Asset = portfolio.Result.Asset!.Append(_mapper.Map<AssetModel>(message)).ToArray();

            await _repository.UpdateAsync(portfolio.Result);

            return new OperationResult<bool>() { Result = true };
        }
    }
}
