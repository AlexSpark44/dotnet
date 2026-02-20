namespace Branch.Platform.Infrastructure.Persistence;

public sealed class IdempotencyRecord
{
    public string Key { get; init; } = default!;
    public Guid OrderId { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}
