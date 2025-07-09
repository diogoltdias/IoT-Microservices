using MQTTnet;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using TelemetryIngestionService.Models;

// Setup RabbitMQ
var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
await channel.QueueDeclareAsync(queue: "telemetry", durable: false, exclusive: false, autoDelete: false, arguments: null);

// Setup MQTT client
var mqttFactory = new MqttClientFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .Build();

mqttClient.ApplicationMessageReceivedAsync += async e =>
{
    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    var telemetry = JsonSerializer.Deserialize<Telemetry>(payload);

    if (telemetry != null)
    {
        // Publish to RabbitMQ
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(telemetry));
        await channel.BasicPublishAsync(exchange: "", routingKey: "telemetry", body: body);
        Console.WriteLine($"Received and forwarded telemetry from {telemetry.DeviceId}");
    }
};

await mqttClient.ConnectAsync(options);
await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("iot/telemetry").Build());

Console.WriteLine("Telemetry Ingestion Service running. Press any key to exit.");
Console.ReadKey();

await mqttClient.DisconnectAsync();