using AutoMapper;
using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using TransactionMicroservice.Definitions.Mongodb.Models;
using TransactionMicroservice.Domain.DbBase;
using TransactionMicroservice.Domain.EventsBase;
using TransactionMicroservice.EventsBase;
using TransactionsEvent;

namespace TransactionMicroservice.Definitions.Kafka.Handlers;

public class CandidatesHandler : IEventHandler<Null, CandidatesFoundEvent>
{
    private readonly IRepository<TransactionModel> _repository;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, TransactionCreatedEvent> _eventProducer;
    private readonly ILogger<CandidatesHandler> _logger;

    public CandidatesHandler(IRepository<TransactionModel> repository, IMapper mapper,
        IEventProducer<Null, TransactionCreatedEvent> eventProducer, ILogger<CandidatesHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _eventProducer = eventProducer;
        _logger = logger;
    }

    public void Process(Message<Null, CandidatesFoundEvent> message)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<bool>> ProcessAsync(Message<Null, CandidatesFoundEvent> message)
    {
        var transactionModel = _mapper.Map<TransactionModel>(message.Value);
        transactionModel.CreatedTime = DateTime.UtcNow;

        var addingResult = await _repository.AddAsync(transactionModel);
        
        if (!addingResult.Ok)
        {
            _logger.LogError($"Error in {nameof(CandidatesHandler)}: {addingResult.Error.Message}");

            var result = new OperationResult<bool>();
            result.AddError(addingResult.Error);

            return result;
        }

        var transactionEvent = new TransactionCreatedEvent()
        {
            AskOrder = message.Value.AskCandidate,
            BidOrder = message.Value.BidCandidate
        };

        var produceResult = await _eventProducer.ProduceAsync(null, transactionEvent);


        if (!produceResult.Ok)
        {
            _logger.LogError($"Error in {nameof(CandidatesHandler)}: {produceResult.Error.Message}");
            var result = new OperationResult<bool>();
            result.AddError(produceResult.Error);

            return result;
        }
        
        return new OperationResult<bool>()
        {
            Result = addingResult.Ok
        };
    }
}