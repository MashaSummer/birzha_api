using Calabonga.OperationResults;
using PortfolioMicroService.Domain.EventsBase;
using Confluent.Kafka;
using AutoMapper;
using PortfolioMicroservice.Domain.DbBase;
using PortfolioMicroService.Definitions.Mongodb.Models;

namespace PortfolioMicroService.Definitions.Kafka.Handlers
{
    public class AddUserHandler : IEventHandler<Null, ProductAddEvent>
    {
        private readonly IRepository<UserModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AddUserHandler(IRepository<UserModel> repository, IMapper mapper, ILogger<AddUserHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public void Process(Message<Null, ProductAddEvent> message)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> ProcessAsync(Message<Null, ProductAddEvent> message)
        {
            throw new NotImplementedException();
        }
    }
}
