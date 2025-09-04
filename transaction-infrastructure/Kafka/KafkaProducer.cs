using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.Sqs;

namespace transaction_infrastructure.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly ILogger<KafkaProducer> logger;
        private readonly KafkaProducerOptions kafkaOptions;
        private IProducer<string, string>? producer;
        public KafkaProducer(ILogger<KafkaProducer> logger, KafkaProducerOptions kafkaOptions)
        {
            this.logger = logger;
            this.kafkaOptions = kafkaOptions;
        }

        public void Configure()
        {
            ProducerConfig producerConfig = new ProducerConfig();
            producerConfig.BootstrapServers = kafkaOptions.BootstrapServers;
            producerConfig.AllowAutoCreateTopics = true;
            producerConfig.EnableSslCertificateVerification = kafkaOptions.EnableSsl;
            ProducerBuilder<string, string> builder = new ProducerBuilder<string, string>(producerConfig);
            builder.SetErrorHandler((_, e) => logger.LogError($"Kafka error: {e.Reason}"));
            this.producer = builder.Build();
        }

        public async Task SendMessage(string message, CancellationToken cancellationToken)
        {
            Configure();
            Message<string, string> messageBody = new Message<string, string>();
            messageBody.Key = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            messageBody.Value = message;
            DeliveryResult<string, string> result = await this.producer!.ProduceAsync(kafkaOptions.Topic, messageBody, cancellationToken);
            if (result.Status == PersistenceStatus.NotPersisted)
                throw new Exception($"Message not persisted. Error: {result.Message.Value}");
        }
    }
}
