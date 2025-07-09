using Common;
using DataEnhancementService.Models;

namespace DataEnhancementService.Processors;
public interface ITelemetryProcessor
{
    Task<EnhancedTelemetry> ProcessAsync(Telemetry telemetry);
}