using Common;
using DataStorageService.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DataStorageService.Services;
public class TelemetryConsumer : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly TelemetryRepository _repository;

    public TelemetryConsumer(IConfiguration config, TelemetryRepository repository)
    {
        _repository = repository ?? throw new Exception();
        _factory = new ConnectionFactory { HostName = config["RabbitMQ:Host"] ?? "localhost" } ?? throw new Exception();
        
    }

    public async Task SetupTelemetryConsumer()
    {
        _connection = await _factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.QueueDeclareAsync(queue: "telemetry", durable: false, exclusive: false, autoDelete: false, arguments: null);
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
                await _repository.AddAsync(telemetry);
                Console.WriteLine($"Stored telemetry for {telemetry.DeviceId} at {telemetry.Timestamp}");
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