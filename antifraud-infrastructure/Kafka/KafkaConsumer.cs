using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.Sqs;

namespace antifraud_infrastructure.Kafka
{
    public class KafkaConsumer: IKafkaConsumer
    {
        private KafkaConsumerOptions options;
        private readonly ILogger<KafkaConsumer> logger;
        private IConsumer<string, string>? consumer;
        public KafkaConsumer(KafkaConsumerOptions options, ILogger<KafkaConsumer> logger)
        {
            this.options = options;
            this.logger = logger;
        }
        public void Configure()
        {
            ConsumerConfig config = new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                GroupId = options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                Acks = Acks.All,
                EnableSslCertificateVerification = options.EnableSsl,
            };
            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(config);
            builder.SetErrorHandler((_, e) => logger.LogError($"Kafka error: {e.Reason}"));
            consumer = builder.Build();
            consumer.Subscribe(options.Topic);
        }

        public string ReadMessage(CancellationToken cancellationToken)
        {
            ConsumeResult<string, string>? message = consumer?.Consume(cancellationToken);
            Console.WriteLine("current message: " + message!.Message.Value);
            return message!.Message.Value;
        }
    }
}
