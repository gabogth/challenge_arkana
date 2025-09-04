using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;
using transaction_infrastructure.Persistence;

namespace challenge.tests;

public class TransactionRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static TransactionRepository CreateRepository(ApplicationDbContext context)
    {
        var logger = Mock.Of<ILogger<TransactionRepository>>();
        return new TransactionRepository(context, logger);
    }

    [Fact]
    public async Task CreateTransaction_PersistsEntity()
    {
        using var context = CreateContext();
        var repository = CreateRepository(context);
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);

        var result = await repository.CreateTransaction(transaction, CancellationToken.None);

        Assert.Equal(transaction.Id, result.Id);
        Assert.Single(context.Transactions);
    }

    [Fact]
    public async Task GetTransaction_ReturnsEntity()
    {
        using var context = CreateContext();
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        context.Transactions.Add(transaction);
        context.SaveChanges();
        var repository = CreateRepository(context);

        var result = await repository.GetTransaction(transaction.Id, CancellationToken.None);

        Assert.Equal(transaction.Id, result.Id);
    }

    [Fact]
    public async Task GetTransaction_NotFound_Throws()
    {
        using var context = CreateContext();
        var repository = CreateRepository(context);

        await Assert.ThrowsAsync<Exception>(() => repository.GetTransaction(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task TotalAmountAtDate_ReturnsSum()
    {
        using var context = CreateContext();
        var sourceId = Guid.NewGuid();
        var date = DateTime.UtcNow;
        var t1 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 100m, Status = TransactionStatusEnum.PENDING, CreatedAt = date };
        var t2 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 50m, Status = TransactionStatusEnum.PENDING, CreatedAt = date };
        var t3 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 25m, Status = TransactionStatusEnum.PENDING, CreatedAt = date.AddDays(1) };
        context.Transactions.AddRange(t1, t2, t3);
        context.SaveChanges();
        var repository = CreateRepository(context);

        var result = await repository.TotalAmountAtDate(sourceId, date, CancellationToken.None);

        Assert.Equal(150m, result);
    }
}
