using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace transaction_infrastructure.Persistence
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext Context;
        private readonly ILogger<TransactionRepository> Logger;
        public TransactionRepository(ApplicationDbContext Context, ILogger<TransactionRepository> Logger)
        {
            this.Context = Context;
            this.Logger = Logger;
        }
        public async Task<Transaction> CreateTransaction(Transaction TransactionEntity, CancellationToken CancellationTokenValue)
        {
            try
            {
                await Context.Transactions.AddAsync(TransactionEntity, CancellationTokenValue);
                await Context.SaveChangesAsync();
                Context.Transactions.Attach(TransactionEntity);
                return TransactionEntity;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public async Task<Transaction> GetTransaction(Guid TransactionId, CancellationToken CancellationTokenValue)
        {
            try 
            { 
                Transaction? CurrentTransaction = await Context.Transactions.Where(x => x.Id == TransactionId).FirstOrDefaultAsync(CancellationTokenValue);
                await Context.SaveChangesAsync(CancellationTokenValue);
                if (CurrentTransaction == null)
                    throw new Exception($"Transaction {TransactionId} not exists.");
                return CurrentTransaction;
            }
            catch (Exception Ex)
            {
                Logger.LogError(Ex, Ex.Message);
                throw;
            }
        }

        public async Task<decimal> TotalAmountAtDate(Guid SourceAccountId, DateTime Date, CancellationToken CancellationToken)
        {
            try
            {
                return await Context.Transactions.Where(x => x.SourceAccountId == SourceAccountId && x.CreatedAt == Date)
                    .SumAsync(x => x.Value);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
