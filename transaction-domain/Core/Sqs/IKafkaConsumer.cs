namespace transaction_domain.Core.Sqs
{
    public interface IKafkaConsumer
    {
        void Configure();
        string ReadMessage(CancellationToken cancellationToken);
    }
}
