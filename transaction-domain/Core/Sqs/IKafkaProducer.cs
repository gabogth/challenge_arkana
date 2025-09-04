namespace transaction_domain.Core.Sqs
{
    public interface IKafkaProducer
    {
        Task SendMessage(string message, CancellationToken cancellationToken);
    }
}
