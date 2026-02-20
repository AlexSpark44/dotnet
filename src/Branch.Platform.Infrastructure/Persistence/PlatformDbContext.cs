using Branch.Platform.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Branch.Platform.Infrastructure.Persistence;

public sealed class PlatformDbContext(DbContextOptions<PlatformDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CreatedAtUtc);
            entity.Property(x => x.Currency).HasMaxLength(3);
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.OwnsMany(x => x.Items, items =>
            {
                items.ToTable("order_items");
                items.WithOwner().HasForeignKey("order_id");
                items.HasKey(x => x.Id);
                items.HasIndex(x => x.ProductId);
                items.Property(x => x.Quantity).IsRequired();
                items.OwnsOne(x => x.UnitPrice, p =>
                {
                    p.Property(m => m.Amount).HasColumnName("unit_price_amount").HasPrecision(18, 2);
                    p.Property(m => m.Currency).HasColumnName("unit_price_currency").HasMaxLength(3);
                });
            });
            entity.Ignore(x => x.DomainEvents);
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("outbox_messages");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ProcessedAtUtc);
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.ToTable("idempotency_records");
            entity.HasKey(x => x.Key);
        });
    }
}
