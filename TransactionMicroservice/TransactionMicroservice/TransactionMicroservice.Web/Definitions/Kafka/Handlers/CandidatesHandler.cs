using System.Transactions;
using AutoMapper;
using Calabonga.OperationResults;
using Confluent.Kafka;
using OrdersEvent;
using TransactionMicroservice.Definitions.Mongodb.Models;
using TransactionMicroservice.Domain.DbBase;
using TransactionMicroservice.Domain.EventsBase;
using TransactionMicroservice.EventsBase;

namespace TransactionMicroservice.Definitions.Kafka.Handlers;

public class CandidatesHandler : IEventHandler<Null, CandidatesFoundEvent>
{
    private readonly IRepository<TransactionModel> _repository;
    private readonly IMapper _mapper;
    private readonly IEventProducer<Null, Transaction> _eventProducer;
    private readonly ILogger<CandidatesHandler> _logger;

    public CandidatesHandler(IRepository<TransactionModel> repository, IMapper mapper, IEventProducer<Null, Transaction> eventProducer, ILogger<CandidatesHandler> logger)
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
        var transaction = _mapper.Map<TransactionModel>(message.Value);
        transaction.CreatedTime = DateTime.Now;

        var addingResult = await _repository.AddAsync(transaction);

        if (!addingResult.Ok)
        {
            _logger.LogError($"Error in {nameof(CandidatesHandler)}: {addingResult.Error.Message}");
        }

        return new OperationResult<bool>()
        {
            Result = addingResult.Ok
        };
    }
}