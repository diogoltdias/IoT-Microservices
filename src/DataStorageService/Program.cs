using DataStorageService.Data;
using DataStorageService.Repositories;
using DataStorageService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddScoped<TelemetryRepository>();
builder.Services.AddHostedService<TelemetryConsumer>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal APIs
app.MapGet("/telemetry/{deviceId}", async (TelemetryRepository repo, string deviceId, DateTime? start, DateTime? end) =>
{
    start ??= DateTime.UtcNow.AddHours(-24);
    end ??= DateTime.UtcNow;
    var telemetry = await repo.GetByDeviceAndTimeRangeAsync(deviceId, start.Value, end.Value);
    return telemetry.Count != 0 ? Results.Ok(telemetry) : Results.NotFound();
});

app.Run();