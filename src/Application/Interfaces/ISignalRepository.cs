using ProyectoForex.Domain.Entities;
namespace ProyectoForex.Application.Interfaces;
public interface ISignalRepository
{
    Task AddAsync(Signal s, CancellationToken ct);
}
