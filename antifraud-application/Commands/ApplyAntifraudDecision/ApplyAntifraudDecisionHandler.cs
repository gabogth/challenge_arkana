using MediatR;
using Microsoft.Extensions.Logging;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace antifraud_application.Commands.ApplyAntifraudDecision
{
    public class ApplyAntifraudDecisionHandler : IRequestHandler<ApplyAntifraudDecisionCommand, ApplyAntifraudDecisionDto>
    {
        private readonly ILogger<ApplyAntifraudDecisionHandler> logger;
        private IKafkaConsumer consumer;
        private ITransactionAntiFraudRepository repository;
        public ApplyAntifraudDecisionHandler(
            ILogger<ApplyAntifraudDecisionHandler> logger, 
            ITransactionAntiFraudRepository repository,
            IKafkaConsumer consumer
            )
        {
            this.logger = logger;
            this.consumer = consumer;
            this.repository = repository;
        }
        public async Task<ApplyAntifraudDecisionDto> Handle(ApplyAntifraudDecisionCommand request, CancellationToken cancellationToken)
        {
            consumer.Configure();
            while (!cancellationToken.IsCancellationRequested) 
            {
                string transactionExternalId = consumer.ReadMessage(cancellationToken);
                Guid transactionExternalGuid = new Guid(transactionExternalId);
                Transaction transaction = await repository.GetTransactionById(transactionExternalGuid, cancellationToken);
                decimal amountAcumulated = await repository.TotalAmountAtDate(transaction.SourceAccountId, DateTime.UtcNow, cancellationToken);
                if (transaction.Value > 2000)
                    await repository.UpdateStatus(transactionExternalGuid, TransactionStatusEnum.REJECTED, cancellationToken);
                else if (amountAcumulated > 20000)
                    await repository.UpdateStatus(transactionExternalGuid, TransactionStatusEnum.REJECTED, cancellationToken);
                else
                    await repository.UpdateStatus(transactionExternalGuid, TransactionStatusEnum.APPROVED, cancellationToken);
            }
            return new ApplyAntifraudDecisionDto { Success = true };
        }
    }
}
