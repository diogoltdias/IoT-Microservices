using Common;
using DataEnhancementService.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DataEnhancementService.Services;
public class TelemetryEnhancer : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly ITelemetryProcessor _processor;

    public TelemetryEnhancer(IConfiguration config, ITelemetryProcessor processor)
    {
        _processor = processor ?? throw new Exception();
        _factory = new ConnectionFactory { HostName = config["RabbitMQ:Host"] ?? "localhost" } ?? throw new Exception();

    }

    public async Task SetupTelemetryConsumer()
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: "telemetry", durable: false, exclusive: false, autoDelete: false, arguments: null);
        await _channel.QueueDeclareAsync(queue: "enhanced_telemetry", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_connection == null || _channel == null)
        {
            await SetupTelemetryConsumer();
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var telemetry = JsonSerializer.Deserialize<Telemetry>(message);
            if (telemetry != null)
            {
                var enhanced = await _processor.ProcessAsync(telemetry);
                var enhancedMessage = JsonSerializer.Serialize(enhanced);
                var enhancedBody = Encoding.UTF8.GetBytes(enhancedMessage);
                await _channel.BasicPublishAsync(exchange: "", routingKey: "enhanced_telemetry", body: enhancedBody, cancellationToken: stoppingToken);
                Console.WriteLine($"Enhanced telemetry for {telemetry.DeviceId}: {enhanced.EnhancedValue}");
            }
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };
        await _channel.BasicConsumeAsync(queue: "telemetry", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        await Task.CompletedTask;
        while (!stoppingToken.IsCancellationRequested) { await Task.Delay(1000, stoppingToken); }
    }

    public override void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}