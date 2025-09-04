using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using transaction_domain.Core.TransactionModule.Entities;
using transaction_domain.Core.TransactionModule.Enums;
using antifraud_infrastructure.Persistence;

namespace challenge.tests;

public class TransactionAntiFraudRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static TransactionAntiFraudRepository CreateRepository(ApplicationDbContext context)
    {
        var logger = Mock.Of<ILogger<TransactionAntiFraudRepository>>();
        return new TransactionAntiFraudRepository(context, logger);
    }

    [Fact]
    public async Task GetTransactionById_ReturnsEntity()
    {
        using var context = CreateContext();
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        context.Transactions.Add(transaction);
        context.SaveChanges();
        var repository = CreateRepository(context);

        var result = await repository.GetTransactionById(transaction.Id, CancellationToken.None);

        Assert.Equal(transaction.Id, result.Id);
    }

    [Fact]
    public async Task UpdateStatus_ChangesStatus()
    {
        using var context = CreateContext();
        var transaction = new Transaction(Guid.NewGuid(), Guid.NewGuid(), 1, 100m);
        context.Transactions.Add(transaction);
        context.SaveChanges();
        var repository = CreateRepository(context);

        var result = await repository.UpdateStatus(transaction.Id, TransactionStatusEnum.APPROVED, CancellationToken.None);

        Assert.Equal(TransactionStatusEnum.APPROVED, result.Status);
        Assert.Equal(TransactionStatusEnum.APPROVED, context.Transactions.Find(transaction.Id)!.Status);
    }

    [Fact]
    public async Task UpdateStatus_NotFound_Throws()
    {
        using var context = CreateContext();
        var repository = CreateRepository(context);

        await Assert.ThrowsAsync<Exception>(() => repository.UpdateStatus(Guid.NewGuid(), TransactionStatusEnum.APPROVED, CancellationToken.None));
    }

    [Fact]
    public async Task TotalAmountAtDate_SumsValuesSameDay()
    {
        using var context = CreateContext();
        var sourceId = Guid.NewGuid();
        var date = new DateTime(2024,1,1,10,0,0,DateTimeKind.Utc);
        var t1 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 100m, Status = TransactionStatusEnum.PENDING, CreatedAt = date };
        var t2 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 50m, Status = TransactionStatusEnum.PENDING, CreatedAt = date.AddHours(3) };
        var t3 = new Transaction { Id = Guid.NewGuid(), SourceAccountId = sourceId, TargetAccountId = Guid.NewGuid(), TransferTypeId = 1, Value = 25m, Status = TransactionStatusEnum.PENDING, CreatedAt = date.AddDays(1) };
        context.Transactions.AddRange(t1, t2, t3);
        context.SaveChanges();
        var repository = CreateRepository(context);

        var result = await repository.TotalAmountAtDate(sourceId, date, CancellationToken.None);

        Assert.Equal(150m, result);
    }
}
