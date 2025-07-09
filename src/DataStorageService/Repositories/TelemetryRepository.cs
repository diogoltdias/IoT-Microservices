using Common;
using DataStorageService.Data;
using Microsoft.EntityFrameworkCore;

namespace DataStorageService.Repositories;
public class TelemetryRepository(TelemetryDbContext context)
{
    public async Task AddAsync(Telemetry telemetry)
    {
        await context.Telemetry.AddAsync(telemetry);
        await context.SaveChangesAsync();
    }

    public async Task<List<Telemetry>> GetByDeviceAndTimeRangeAsync(string deviceId, DateTime start, DateTime end)
    {
        return await context.Telemetry
            .Where(t => t.DeviceId == deviceId && t.Timestamp >= start && t.Timestamp <= end)
            .OrderBy(t => t.Timestamp)
            .ToListAsync();
    }
}