namespace ProyectoForex.Application.Interfaces;
public interface IRiskGuard
{
    Task EnsureAllowedAsync(string ticker, decimal price, string action, CancellationToken ct);
}
