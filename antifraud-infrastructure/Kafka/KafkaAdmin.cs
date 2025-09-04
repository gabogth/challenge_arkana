using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.Sqs;

namespace antifraud_infrastructure.Kafka
{
    public class KafkaAdmin : IKafkaAdmin
    {
        private KafkaConsumerOptions options;
        private readonly ILogger<KafkaConsumer> logger;
        private IAdminClient? adminClient;
        public KafkaAdmin(KafkaConsumerOptions options, ILogger<KafkaConsumer> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public void Configure()
        {
            AdminClientConfig config = new AdminClientConfig
            {
                BootstrapServers = options.BootstrapServers,
                EnableSslCertificateVerification = options.EnableSsl
            };
            AdminClientBuilder builder = new AdminClientBuilder(config);
            adminClient = builder.Build();
        }

        public async Task CreateTopic(string topicName)
        {
            try
            {
                Configure();
                await adminClient.CreateTopicsAsync(new TopicSpecification[]
                {
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
                });
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    throw;
            }
        }
    }
}
