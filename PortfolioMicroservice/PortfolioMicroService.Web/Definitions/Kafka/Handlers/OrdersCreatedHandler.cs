using Calabonga.OperationResults;
using PortfolioMicroService.Domain.EventsBase;
using Confluent.Kafka;
using AutoMapper;
using PortfolioMicroservice.Domain.DbBase;
using PortfolioMicroService.Definitions.Mongodb.Models;
using PortfolioMicroService.EventsBase;

namespace PortfolioMicroService.Definitions.Kafka.Handlers
{
    public class OrdersCreatedHandler : IEventHandler<Null, OrderCreatedEvent>
    {
        private readonly IRepository<UserModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IEventProducer<Null, OrderValidationEvent> _eventProducer;

        public OrdersCreatedHandler(IRepository<UserModel> repository, IMapper mapper, ILogger<OrdersCreatedHandler> logger, IEventProducer<Null, OrderValidationEvent> eventProducer)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _eventProducer = eventProducer;

        }

        public void Process(Message<Null, OrderCreatedEvent> message)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> ProcessAsync(Message<Null, OrderCreatedEvent> message)
        {
            var portfolio = await _repository.GetByIdAsync(message.Value.Order.InvestorId);
            var index = 0;
            var Asset = portfolio.Result.Asset.Where((x, _index) =>
            {
                index = _index;
                return x.Id == message.Value.Order.ProductId;                   
            }).First();
            var producerEvent = new OrderValidationEvent() { OrderId = message.Value.OrderId};


            if (Asset.VolumeActive < message.Value.Order.Volume)
            {
                producerEvent.Valid = false;
            }
            else
            {                
                Asset.VolumeActive -= message.Value.Order.Volume;
                Asset.VolumeFrozen += message.Value.Order.Volume;
                portfolio.Result.Asset[index] = Asset;
                await _repository.UpdateAsync(portfolio.Result);
                producerEvent.Valid = true;
            }

            var produceResult = await _eventProducer.ProduceAsync(null,producerEvent);
            if (!produceResult.Ok)
            {
                _logger.LogError($"Error in {nameof(OrdersCreatedHandler)}: {produceResult.Error.Message}");
                var result = new OperationResult<bool>();
                result.AddError(produceResult.Error);

                return result;
            }

            return new OperationResult<bool>() { Result = true };
        }
    }
}
