using MediatR;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Interfaces;

namespace transaction_application.Commands.CreateTransaction
{
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionDto>
    {
        private readonly ITransactionRepository repository;
        private readonly IKafkaProducer sender;
        public CreateTransactionHandler(ITransactionRepository repository, IKafkaProducer sender)
        {
            this.repository = repository;
            this.sender = sender;
        }
        public async Task<CreateTransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            Transaction entity = new Transaction(
                request.SourceAccountId,
                request.TargetAccountId, 
                request.TransferTypeId, 
                request.Value
            );
            Transaction transactionInserted = await repository.CreateTransaction(entity, cancellationToken);
            await sender.SendMessage(transactionInserted.Id.ToString(), cancellationToken);
            return new CreateTransactionDto
            {
                TransactionExternalId = transactionInserted.Id,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
