using Moq;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Interfaces;
using transaction_application.Queries.GetTransaction;
using Xunit;

namespace challenge.tests;

public class GetTransactionHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTransactionDto()
    {
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        var repository = new Mock<ITransactionRepository>();
        repository.Setup(r => r.GetTransaction(transaction.Id, It.IsAny<CancellationToken>())).ReturnsAsync(transaction);

        var handler = new GetTransactionHandler(repository.Object);
        var query = new GetTransactionQuery(transaction.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(transaction.Id, result.TransactionExternalId);
        Assert.Equal(transaction.SourceAccountId, result.SourceAccountId);
        Assert.Equal(transaction.TargetAccountId, result.TargetAccountId);
        Assert.Equal(transaction.TransferTypeId, result.TransferTypeId);
        Assert.Equal(transaction.Value, result.Value);
        Assert.Equal(transaction.Status, result.Status);
        Assert.Equal(transaction.CreatedAt, result.CreatedAt);
    }
}
