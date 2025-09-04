using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Interfaces;
using transaction_infrastructure.Kafka;
using transaction_infrastructure.Persistence;

namespace transaction_application
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IKafkaProducer, KafkaProducer>();
            services.AddSingleton(new KafkaProducerOptions() { 
                BootstrapServers = configuration.GetSection("kafka:bootstrapServers").Value!,
                EnableSsl = bool.Parse(configuration.GetSection("kafka:enableSsl").Value!),
                Topic = configuration.GetSection("kafka:topic").Value!
            });
            return services;
        }
    }
}
