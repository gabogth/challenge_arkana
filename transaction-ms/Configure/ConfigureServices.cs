using FluentValidation;
using Microsoft.EntityFrameworkCore;
using transaction_application;
using transaction_application.Behaviors;
using transaction_application.Commands.CreateTransaction;
using transaction_infrastructure.Persistence;

namespace transaction_ms.Configure
{
    public static class ConfigureServices
    {
        public static IServiceCollection Configure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.ConfigureInfrastructure(configuration);
            ConfigureValidation(services, configuration);
            ConfigureDatabase(services, configuration);
            ConfigureCommandArchitect(services);
            return services;
        }

        private static void ConfigureValidation(IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddValidatorsFromAssembly(typeof(CreateTransactionValidator).Assembly);
            services.AddMediatR(cnf => {
                cnf.RegisterServicesFromAssemblyContaining(typeof(CreateTransactionValidator));
                cnf.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                string con = configuration.GetConnectionString("DefaultConnectionString")! ?? "empty";
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString"), b => b.MigrationsAssembly("transaction-ms"));
            });
        }

        private static void ConfigureCommandArchitect(IServiceCollection services)
        {
            services.AddMediatR(x => {
                x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
                x.RegisterServicesFromAssemblies(typeof(CreateTransactionHandler).Assembly);
            });
        }

    }
}
