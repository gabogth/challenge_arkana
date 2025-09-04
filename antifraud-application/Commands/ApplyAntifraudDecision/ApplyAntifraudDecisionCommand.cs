using MediatR;

namespace antifraud_application.Commands.ApplyAntifraudDecision
{
    public record ApplyAntifraudDecisionCommand(
        CancellationToken stoppingToken
    ) : IRequest<ApplyAntifraudDecisionDto>;
}
