using ProyectoForex.Application.Interfaces;
using ProyectoForex.Domain.Entities;
using ProyectoForex.Infrastructure.Data;

namespace ProyectoForex.Infrastructure.Persistence;
public sealed class SignalRepository : ISignalRepository
{
    private readonly ForexDbContext _db;
    public SignalRepository(ForexDbContext db) => _db = db;
    public async Task AddAsync(Signal s, CancellationToken ct)
    {
        _db.Signals.Add(s);
        await _db.SaveChangesAsync(ct);
    }
}
