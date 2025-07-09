namespace Common;
public class Device
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "Offline";
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
