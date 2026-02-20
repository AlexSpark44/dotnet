using Branch.Platform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Branch.Platform.Infrastructure.Outbox;

public sealed class OutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
            var messages = await db.OutboxMessages.Where(x => x.ProcessedAtUtc == null).Take(50).ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                logger.LogInformation("Dispatching outbox message {MessageId} ({Type})", message.Id, message.Type);
                message.ProcessedAtUtc = DateTime.UtcNow;
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
