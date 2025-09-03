using ProyectoForex.Application.Interfaces;
namespace ProyectoForex.Infrastructure.Time;
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
