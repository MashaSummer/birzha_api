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
            
            try
            {

            if (!portfolio.Ok) 
            {
                _logger.LogError("Investor id ({0}) not found", message.Value.InvestorId);

                portfolio = OperationResult.CreateResult(new UserModel() { Id = message.Value.InvestorId });
                await _repository.AddAsync(portfolio.Result);

                _logger.LogInformation("Created investor record for id: {0}", message.Value.InvestorId);
            }

            portfolio.Result.Asset = portfolio.Result.Asset!.Append(_mapper.Map<AssetModel>(message.Value)).ToArray();

            await _repository.UpdateAsync(portfolio.Result);
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return new OperationResult<bool>() { Result = true };
        }
    }
}
