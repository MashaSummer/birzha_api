using AuthMicroservice.Web.Definitions.Mediator.Base;
using AuthMicroservice.Web.Endpoints.EventItemsEndpoints.ViewModels;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;

namespace AuthMicroservice.Web.Definitions.Mediator;

public class EventItemPostTransactionBehavior : TransactionBehavior<IRequest<OperationResult<EventItemViewModel>>, OperationResult<EventItemViewModel>>
{
    public EventItemPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
}