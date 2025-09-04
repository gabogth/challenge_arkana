using transaction_domain.Core.TransactionModule.Enums;

namespace transaction_domain.Core.TransactionModule.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public int TransferTypeId { get; set; }
        public decimal Value { get; set; }
        public TransactionStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Transaction()
        {
            
        }

        public Transaction(Guid source, Guid target, int transferTypeId, decimal value)
        {
            Id = Guid.NewGuid();
            SourceAccountId = source;
            TargetAccountId = target;
            TransferTypeId = transferTypeId;
            Value = value;
            Status = TransactionStatusEnum.PENDING;
            CreatedAt = DateTime.UtcNow;
        }

        public void Approve()
        {
            if (Status != TransactionStatusEnum.PENDING) return;
            Status = TransactionStatusEnum.APPROVED;
        }

        public void Reject()
        {
            if (Status != TransactionStatusEnum.PENDING) return;
            Status = TransactionStatusEnum.REJECTED;
        }
    }
}
