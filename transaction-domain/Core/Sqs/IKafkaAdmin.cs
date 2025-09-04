namespace transaction_domain.Core.Sqs
{
    public interface IKafkaAdmin
    {
        Task CreateTopic(string topicName);
    }
}
