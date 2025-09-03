using MediatR;
using ProyectoForex.Domain.Entities;
using ProyectoForex.Application.Interfaces;

namespace ProyectoForex.Application.Signals;
public sealed class CreateSignalHandler : IRequestHandler<CreateSignalCommand, Guid>
{
    private readonly ISignalRepository _repo;
    private readonly IAlertDeduper _deduper;
    private readonly IClock _clock;
    private readonly IRiskGuard _risk;

    public CreateSignalHandler(ISignalRepository repo, IAlertDeduper deduper, IClock clock, IRiskGuard risk)
    {
        _repo = repo; _deduper = deduper; _clock = clock; _risk = risk;
    }

    public async Task<Guid> Handle(CreateSignalCommand request, CancellationToken ct)
    {
        var d = request.Dto;
        var key = $"{d.Ticker}:{d.Action}:{d.Ts}:{d.Price}";
        var isNew = await _deduper.TryLockAsync(key, TimeSpan.FromMinutes(5), ct);
        if (!isNew) return Guid.Empty;

        await _risk.EnsureAllowedAsync(d.Ticker, d.Price, d.Action, ct);

        var entity = new Signal
        {
            Ticker = d.Ticker,
            Price = d.Price,
            Action = d.Action,
            Ts = d.Ts,
            Exchange = d.Exchange,
            Volume = d.Volume,
            ReceivedAtUtc = _clock.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }
}
