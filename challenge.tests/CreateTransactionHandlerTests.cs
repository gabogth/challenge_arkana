using Moq;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Interfaces;
using transaction_application.Commands.CreateTransaction;

namespace challenge.tests;

public class CreateTransactionHandlerTests
{
    [Fact]
    public async Task Handle_CreatesTransactionAndSendsMessage()
    {
        var repository = new Mock<ITransactionRepository>();
        Transaction? saved = null;
        repository.Setup(r => r.CreateTransaction(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                  .Callback<Transaction, CancellationToken>((t, _) => saved = t)
                  .ReturnsAsync((Transaction t, CancellationToken _) => t);

        var producer = new Mock<IKafkaProducer>();
        var handler = new CreateTransactionHandler(repository.Object, producer.Object);

        var command = new CreateTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(saved);
        Assert.Equal(saved!.Id, result.TransactionExternalId);
        Assert.Equal(saved.CreatedAt, result.CreatedAt);
        producer.Verify(p => p.SendMessage(saved.Id.ToString(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
