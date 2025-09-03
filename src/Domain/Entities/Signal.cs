namespace ProyectoForex.Domain.Entities;
public sealed class Signal
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Ticker { get; init; } = default!;
    public decimal Price { get; init; }
    public string Action { get; init; } = default!; // buy/sell/close/hold
    public long Ts { get; init; }
    public DateTime ReceivedAtUtc { get; init; } = DateTime.UtcNow;
    public string? Exchange { get; init; }
    public decimal? Volume { get; init; }
}
