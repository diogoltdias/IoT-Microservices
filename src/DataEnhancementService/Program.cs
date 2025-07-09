using DataEnhancementService.Processors;
using DataEnhancementService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<TelemetryEnhancer>();
    services.AddSingleton<ITelemetryProcessor, MovingAverageProcessor>();
});

await builder.Build().RunAsync();