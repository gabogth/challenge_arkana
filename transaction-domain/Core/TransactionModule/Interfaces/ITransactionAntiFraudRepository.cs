using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;

namespace transaction_domain.Core.TransactionModule.Interfaces
{
    public interface ITransactionAntiFraudRepository
    {
        Task<Transaction> GetTransactionById(Guid TransactionExternalId, CancellationToken CancellationToken);
        Task<decimal> TotalAmountAtDate(Guid SourceAccountId, DateTime Date, CancellationToken CancellationToken);
        Task<Transaction> UpdateStatus(Guid TransactionExternalId, TransactionStatusEnum Status, CancellationToken CancellationToken);
    }
}
