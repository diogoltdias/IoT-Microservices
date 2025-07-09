using DeviceRegistryService.Models;
using DeviceRegistryService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<DeviceService>();
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
app.MapGet("/devices", async (DeviceService service) =>
    await service.GetAllAsync());

app.MapGet("/devices/{id}", async (DeviceService service, string id) =>
    await service.GetByIdAsync(id) is Device device ? Results.Ok(device) : Results.NotFound());

app.MapPost("/devices", async (DeviceService service, Device device) =>
{
    var created = await service.CreateAsync(device);
    return Results.Created($"/devices/{created.Id}", created);
});

app.MapPut("/devices/{id}", async (DeviceService service, string id, Device device) =>
{
    await service.UpdateAsync(id, device);
    return Results.NoContent();
});

app.MapDelete("/devices/{id}", async (DeviceService service, string id) =>
{
    await service.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();