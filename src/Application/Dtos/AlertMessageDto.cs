namespace ProyectoForex.Application.Dtos;
public sealed record AlertMessageDto(
    string Secret,
    string Ticker,
    decimal Price,
    string Action,
    long Ts,
    decimal? Volume,
    string? Exchange);
