using transaction_domain.Core.TransactionModule.Enums;

namespace transaction_application.Queries.GetTransaction
{
    public class GetTransactionDto
    {
        public Guid TransactionExternalId { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
