using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Common;

var builder = WebApplication.CreateBuilder(args);

// Optional JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "supersecretkey1234567890"))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
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
app.UseAuthentication();
app.UseAuthorization();

// Minimal APIs
app.MapGet("/devices", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient();
    var response = await client.GetFromJsonAsync<List<Device>>("http://device-registry:80/devices");
    return response ?? [];
}).RequireAuthorization();

app.MapGet("/devices/{id}", async (IHttpClientFactory clientFactory, string id) =>
{
    var client = clientFactory.CreateClient();
    var response = await client.GetFromJsonAsync<Device>($"http://device-registry:80/devices/{id}");
    return response is not null ? Results.Ok(response) : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/telemetry/{deviceId}", async (IHttpClientFactory clientFactory, string deviceId, DateTime? start, DateTime? end) =>
{
    var client = clientFactory.CreateClient();
    start ??= DateTime.UtcNow.AddHours(-24);
    end ??= DateTime.UtcNow;
    var response = await client.GetFromJsonAsync<List<Telemetry>>(
        $"http://data-storage:80/telemetry/{deviceId}?start={start:yyyy-MM-ddTHH:mm:ssZ}&end={end:yyyy-MM-ddTHH:mm:ssZ}");
    return response is not null ? Results.Ok(response) : Results.NotFound();
}).RequireAuthorization();

app.Run();