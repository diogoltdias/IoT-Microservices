using Common;
using DataEnhancementService.Models;
using System.Collections.Concurrent;

namespace DataEnhancementService.Processors;
public class MovingAverageProcessor(int windowSize = 5) : ITelemetryProcessor
{
    private readonly ConcurrentDictionary<string, Queue<double>> _deviceValues = new();

    public async Task<EnhancedTelemetry> ProcessAsync(Telemetry telemetry)
    {
        var queue = _deviceValues.GetOrAdd(telemetry.DeviceId, new Queue<double>());
        queue.Enqueue(telemetry.Value);
        if (queue.Count > windowSize) queue.Dequeue();

        var average = queue.Average();
        var stdDev = Math.Sqrt(queue.Average(v => Math.Pow(v - average, 2)));
        var zScore = (telemetry.Value - average) / (stdDev == 0 ? 1 : stdDev);
        var isAnomaly = Math.Abs(zScore) > 2; // Simple anomaly detection

        return await Task.FromResult(new EnhancedTelemetry
        {
            DeviceId = telemetry.DeviceId,
            OriginalValue = telemetry.Value,
            EnhancedValue = average,
            Type = telemetry.Type,
            Timestamp = telemetry.Timestamp,
            IsAnomaly = isAnomaly
        });
    }
}