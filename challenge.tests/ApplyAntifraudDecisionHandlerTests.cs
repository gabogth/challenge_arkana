using Microsoft.Extensions.Logging;
using Moq;
using transaction_domain.Core.Sqs;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;
using transaction_domain.Core.TransactionModule.Interfaces;
using antifraud_application.Commands.ApplyAntifraudDecision;
using Xunit;

namespace challenge.tests;

public class ApplyAntifraudDecisionHandlerTests
{
    [Fact]
    public async Task Handle_ValueGreaterThan2000_Rejects()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 3000m);
        var cts = new CancellationTokenSource();

        var consumer = new Mock<IKafkaConsumer>();
        consumer.Setup(c => c.Configure());
        consumer.Setup(c => c.ReadMessage(It.IsAny<CancellationToken>())).Returns(transaction.Id.ToString());

        var repository = new Mock<ITransactionAntiFraudRepository>();
        repository.Setup(r => r.GetTransactionById(transaction.Id, It.IsAny<CancellationToken>())).ReturnsAsync(transaction);
        repository.Setup(r => r.TotalAmountAtDate(transaction.SourceAccountId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(0m);
        repository.Setup(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.REJECTED, It.IsAny<CancellationToken>()))
                  .Callback(() => cts.Cancel())
                  .ReturnsAsync(transaction);

        var handler = new ApplyAntifraudDecisionHandler(Mock.Of<ILogger<ApplyAntifraudDecisionHandler>>(), repository.Object, consumer.Object);

        var result = await handler.Handle(new ApplyAntifraudDecisionCommand(cts.Token), cts.Token);

        Assert.True(result.Success);
        repository.Verify(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.REJECTED, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AccumulatedAmountGreaterThan20000_Rejects()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        var cts = new CancellationTokenSource();

        var consumer = new Mock<IKafkaConsumer>();
        consumer.Setup(c => c.Configure());
        consumer.Setup(c => c.ReadMessage(It.IsAny<CancellationToken>())).Returns(transaction.Id.ToString());

        var repository = new Mock<ITransactionAntiFraudRepository>();
        repository.Setup(r => r.GetTransactionById(transaction.Id, It.IsAny<CancellationToken>())).ReturnsAsync(transaction);
        repository.Setup(r => r.TotalAmountAtDate(transaction.SourceAccountId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(25000m);
        repository.Setup(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.REJECTED, It.IsAny<CancellationToken>()))
                  .Callback(() => cts.Cancel())
                  .ReturnsAsync(transaction);

        var handler = new ApplyAntifraudDecisionHandler(Mock.Of<ILogger<ApplyAntifraudDecisionHandler>>(), repository.Object, consumer.Object);

        var result = await handler.Handle(new ApplyAntifraudDecisionCommand(cts.Token), cts.Token);

        Assert.True(result.Success);
        repository.Verify(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.REJECTED, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Default_Approves()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        var cts = new CancellationTokenSource();

        var consumer = new Mock<IKafkaConsumer>();
        consumer.Setup(c => c.Configure());
        consumer.Setup(c => c.ReadMessage(It.IsAny<CancellationToken>())).Returns(transaction.Id.ToString());

        var repository = new Mock<ITransactionAntiFraudRepository>();
        repository.Setup(r => r.GetTransactionById(transaction.Id, It.IsAny<CancellationToken>())).ReturnsAsync(transaction);
        repository.Setup(r => r.TotalAmountAtDate(transaction.SourceAccountId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(1000m);
        repository.Setup(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.APPROVED, It.IsAny<CancellationToken>()))
                  .Callback(() => cts.Cancel())
                  .ReturnsAsync(transaction);

        var handler = new ApplyAntifraudDecisionHandler(Mock.Of<ILogger<ApplyAntifraudDecisionHandler>>(), repository.Object, consumer.Object);

        var result = await handler.Handle(new ApplyAntifraudDecisionCommand(cts.Token), cts.Token);

        Assert.True(result.Success);
        repository.Verify(r => r.UpdateStatus(transaction.Id, TransactionStatusEnum.APPROVED, It.IsAny<CancellationToken>()), Times.Once);
    }
}
