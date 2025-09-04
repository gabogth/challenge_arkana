using MediatR;
using transaction_application.Commands.CreateTransaction;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace transaction_application.Queries.GetTransaction
{
    public class GetTransactionHandler : IRequestHandler<GetTransactionQuery, GetTransactionDto>
    {
        private readonly ITransactionRepository repository;
        public GetTransactionHandler(ITransactionRepository repository)
        {
            this.repository = repository;
        }
        public async Task<GetTransactionDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            Transaction transaction = await repository.GetTransaction(request.TransactionExternalId, cancellationToken);
            return new GetTransactionDto
            {
                TransactionExternalId = transaction.Id,
                SourceAccountId = transaction.SourceAccountId,
                TargetAccountId = transaction.TargetAccountId,
                TransferTypeId = transaction.TransferTypeId,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status,
                Value = transaction.Value
            };
        }
    }
}
