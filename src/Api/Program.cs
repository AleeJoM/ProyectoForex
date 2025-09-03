using System.Text.Json;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProyectoForex.Application.Dtos;
using ProyectoForex.Application.Interfaces;
using ProyectoForex.Application.Signals;
using ProyectoForex.Infrastructure.Data;
using ProyectoForex.Infrastructure.Persistence;
using ProyectoForex.Infrastructure.Redis;
using ProyectoForex.Infrastructure.Risk;
using ProyectoForex.Infrastructure.Time;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddDbContext<ForexDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres") ?? "Host=localhost;Port=5432;Database=forexdb;Username=forex;Password=forex"));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<ISignalRepository, SignalRepository>();
builder.Services.AddSingleton<IAlertDeduper, AlertDeduper>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IRiskGuard, AllowAllRiskGuard>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateSignalCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateSignalValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Text("API OK", "text/plain"));
app.MapGet("/health", () => Results.Ok(new { status = "ok", ts = DateTimeOffset.UtcNow }));
app.MapGet("/ready", (ForexDbContext db) => Results.Ok(new { ready = true }));

app.MapPost("/webhooks/tradingview", async (HttpRequest req, IConfiguration cfg, IMediator mediator, IValidator<CreateSignalCommand> validator) =>
{
    using var reader = new StreamReader(req.Body);
    var body = await reader.ReadToEndAsync();
    AlertMessageDto? dto = null;
    try
    {
        dto = JsonSerializer.Deserialize<AlertMessageDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch { return Results.BadRequest(new { error = "Invalid JSON" }); }
    if (dto is null) return Results.BadRequest(new { error = "Invalid payload" });

    var expected = cfg["TV_SHARED_SECRET"];
    if (string.IsNullOrWhiteSpace(expected)) return Results.Problem("TV_SHARED_SECRET not configured");
    if (!string.Equals(dto.Secret, expected, StringComparison.Ordinal)) return Results.Unauthorized();

    var cmd = new CreateSignalCommand(dto);
    var validation = await validator.ValidateAsync(cmd);
    if (!validation.IsValid) return Results.BadRequest(new { error = validation.Errors.Select(e => e.ErrorMessage) });

    var id = await mediator.Send(cmd);
    return Results.Ok(new { received = true, id });
})
.WithName("TradingViewWebhook");

app.Run();
