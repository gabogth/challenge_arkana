namespace transaction_domain.Core.Sqs
{
    public class KafkaProducerOptions
    {
        public string BootstrapServers { get; set; } = default!;
        public string Topic { get; set; } = default!;
        public bool EnableSsl { get; set; }
    }
}
