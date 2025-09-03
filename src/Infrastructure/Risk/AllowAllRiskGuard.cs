using ProyectoForex.Application.Interfaces;
namespace ProyectoForex.Infrastructure.Risk;
public sealed class AllowAllRiskGuard : IRiskGuard
{
    public Task EnsureAllowedAsync(string ticker, decimal price, string action, CancellationToken ct)
        => Task.CompletedTask;
}
