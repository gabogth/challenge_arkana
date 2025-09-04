namespace transaction_domain.Core.Sqs
{
    public interface IKafkaConsumer
    {
        Task Configure();
        string ReadMessage(CancellationToken cancellationToken);
    }
}
