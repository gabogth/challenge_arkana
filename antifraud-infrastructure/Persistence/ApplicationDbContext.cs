using Microsoft.EntityFrameworkCore;
using transaction_domain.Core.TransactionModule.Entities;

namespace antifraud_infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("transactions");
                e.HasKey(x => x.Id);

                e.Property(x => x.SourceAccountId)
                    .HasColumnName("source_account_id")
                    .IsRequired();

                e.Property(x => x.TargetAccountId)
                    .HasColumnName("target_account_id")
                    .IsRequired();

                e.Property(x => x.TransferTypeId)
                    .HasColumnName("transfer_type_id")
                    .IsRequired();

                e.Property(x => x.Value)
                    .HasColumnName("value")
                    .HasColumnType("numeric(18,2)")
                    .IsRequired();

                e.Property(x => x.Status)
                    .HasColumnName("status")
                    .HasConversion<int>()
                    .IsRequired();

                e.Property(x => x.CreatedAt)
                    .HasColumnName("created_at");

                e.HasIndex(x => x.CreatedAt);
                e.HasIndex(x => new { x.SourceAccountId, x.CreatedAt });
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
