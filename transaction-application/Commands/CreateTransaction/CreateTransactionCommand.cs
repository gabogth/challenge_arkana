using MediatR;
using transaction_application.Interfaces;

namespace transaction_application.Commands.CreateTransaction
{
    public record CreateTransactionCommand(
        Guid SourceAccountId,
        Guid TargetAccountId,
        int TransferTypeId,
        decimal Value
    ) : IRequest<CreateTransactionDto>, ICommandBase;
}
