using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;
using ProductMicroservice.Web.Definitions.Mediator.Base;
using ProductMicroservice.Web.Endpoints.EventItemsEndpoints.ViewModels;

namespace ProductMicroservice.Web.Definitions.Mediator
{
    public class LogPostTransactionBehavior : TransactionBehavior<IRequest<OperationResult<EventItemViewModel>>, OperationResult<EventItemViewModel>>
    {
        public LogPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}