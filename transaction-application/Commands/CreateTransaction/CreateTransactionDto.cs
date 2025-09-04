namespace transaction_application.Commands.CreateTransaction
{
    public class CreateTransactionDto
    {
        public Guid TransactionExternalId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
