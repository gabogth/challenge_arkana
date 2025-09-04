using antifraud_infrastructure.Kafka;
using antifraud_infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace antifraud_application
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddTransient<IKafkaConsumer, KafkaConsumer>();
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                string con = configuration.GetConnectionString("DefaultConnectionString")! ?? "empty";
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"));
            });
            services.AddTransient<ITransactionAntiFraudRepository, TransactionAntiFraudRepository>();
            
            services.AddSingleton(new KafkaConsumerOptions()
            {
                BootstrapServers = configuration.GetSection("kafka:bootstrapServers").Value!,
                EnableSsl = bool.Parse(configuration.GetSection("kafka:enableSsl").Value!),
                Topic = configuration.GetSection("kafka:topic").Value!,
                GroupId = configuration.GetSection("kafka:groupId").Value!,
            });
            return services;
        }
    }
}
