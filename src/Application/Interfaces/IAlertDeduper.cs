namespace ProyectoForex.Application.Interfaces;
public interface IAlertDeduper
{
    Task<bool> TryLockAsync(string key, TimeSpan ttl, CancellationToken ct);
}
