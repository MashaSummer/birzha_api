using AutoMapper;
using Calabonga.OperationResults;
using Confluent.Kafka;
using NewPortfolioMicroservice.Definitions.Mongodb.Models;
using NewPortfolioMicroservice.Domain.DbBase;
using NewPortfolioMicroservice.Domain.EventsBase;
using PortfolioMicroService;

namespace NewPortfolioMicroservice.Definitions.Kafka.Handlers
{
    public class AuthRegisterHandler : IEventHandler<Null, UserRegisterEvent>
    {
        private readonly IRepository<UserModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AuthRegisterHandler(IRepository<UserModel> repository, IMapper mapper, ILogger<AuthRegisterHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public void Process(Message<Null, UserRegisterEvent> message)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> ProcessAsync(Message<Null, UserRegisterEvent> message)
        {
            await _repository.AddAsync(new UserModel() { Id = message.Value.InvestorId});

            return new OperationResult<bool>() { Result = true };
        }
    }
}
