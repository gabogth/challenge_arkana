using antifraud_application.Commands.ApplyAntifraudDecision;
using MediatR;

namespace antifraud_worker.Consumers
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly ILogger<KafkaConsumerWorker> logger;
        private readonly IMediator mediator;
        private readonly IServiceScopeFactory scopeFactory;

        public KafkaConsumerWorker(IMediator mediator, ILogger<KafkaConsumerWorker> logger, IServiceScopeFactory scopeFactory)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(new ApplyAntifraudDecisionCommand(stoppingToken));
        }
    }
}
