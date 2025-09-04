using MediatR;
using transaction_application.Interfaces;

namespace transaction_application.Queries.GetTransaction
{
    public record GetTransactionQuery(
        Guid TransactionExternalId
    ) : IRequest<GetTransactionDto>, ICommandBase;
}
