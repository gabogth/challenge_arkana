using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace antifraud_infrastructure.Persistence
{
    public class TransactionAntiFraudRepository : ITransactionAntiFraudRepository
    {
        private readonly ApplicationDbContext Context;
        private readonly ILogger<TransactionAntiFraudRepository> Logger;
        public TransactionAntiFraudRepository(ApplicationDbContext Context, ILogger<TransactionAntiFraudRepository> Logger)
        {
            this.Context = Context;
            this.Logger = Logger;
        }
        public async Task<Transaction> GetTransactionById(Guid TransactionExternalId, CancellationToken CancellationToken)
        {
            try
            {
                return await Context.Transactions.Where(x => x.Id == TransactionExternalId).FirstOrDefaultAsync(CancellationToken);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public async Task<Transaction> UpdateStatus(Guid TransactionExternalId, TransactionStatusEnum Status, CancellationToken CancellationToken)
        {
            try
            {
                Transaction transaction = await Context.Transactions.Where(x => x.Id == TransactionExternalId).FirstOrDefaultAsync(CancellationToken);
                if (transaction == null)
                    throw new Exception($"Transaction {TransactionExternalId} not exists.");
                transaction.Status = Status;
                await Context.SaveChangesAsync(CancellationToken);
                return transaction;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<decimal> TotalAmountAtDate(Guid SourceAccountId, DateTime Date, CancellationToken CancellationToken)
        {
            try
            {
                return await Context.Transactions.Where(x => x.SourceAccountId == SourceAccountId && x.CreatedAt.Date == Date.Date)
                    .SumAsync(x => x.Value, CancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
