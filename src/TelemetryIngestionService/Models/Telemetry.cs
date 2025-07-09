namespace TelemetryIngestionService.Models;
public class Telemetry
{
    public string DeviceId { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Type { get; set; } = string.Empty; // e.g., "temperature"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}