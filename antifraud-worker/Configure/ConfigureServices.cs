using antifraud_application;
using antifraud_application.Commands.ApplyAntifraudDecision;
using antifraud_infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace antifraud_worker.Configure
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.ConfigureInfrastructure(configuration);
            services.ConfigureDatabase(configuration);
            services.ConfigureCommandArchitect();
            return services;
        }

        private static void ConfigureDatabase(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString"));
            });
        }

        private static void ConfigureCommandArchitect(this IServiceCollection services)
        {
            services.AddMediatR(x => {
                x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
                x.RegisterServicesFromAssemblies(typeof(ApplyAntifraudDecisionHandler).Assembly);
            });
        }
    }
}
