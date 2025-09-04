using transaction_domain.Core.TransactionModule.Entities;

namespace transaction_domain.Core.TransactionModule.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateTransaction(Transaction transaction, CancellationToken cancellationToken);
        Task<Transaction> GetTransaction(Guid transactionId, CancellationToken cancellationToken);
    }
}
