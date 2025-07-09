namespace DataEnhancementService.Models;
public class EnhancedTelemetry
{
    public string DeviceId { get; set; } = string.Empty;
    public double OriginalValue { get; set; }
    public double EnhancedValue { get; set; } // e.g., moving average
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public bool IsAnomaly { get; set; }
}