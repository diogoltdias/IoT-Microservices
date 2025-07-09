using Common;
using Microsoft.EntityFrameworkCore;

namespace DataStorageService.Data;
public class TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : DbContext(options)
{
    public DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Telemetry>()
            .HasKey(t => new { t.Timestamp, t.DeviceId });
    }
}