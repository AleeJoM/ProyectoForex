using ProyectoForex.Application.Interfaces;
using StackExchange.Redis;

namespace ProyectoForex.Infrastructure.Redis;
public sealed class AlertDeduper : IAlertDeduper
{
    private readonly IDatabase _db;
    public AlertDeduper(IConnectionMultiplexer mux) => _db = mux.GetDatabase();
    public async Task<bool> TryLockAsync(string key, TimeSpan ttl, CancellationToken ct)
        => await _db.StringSetAsync($"dedupe:{key}", "1", ttl, When.NotExists);
}
