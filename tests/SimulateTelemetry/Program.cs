using MQTTnet;
using System.Text;
using System.Text.Json;

var mqttFactory = new MqttClientFactory();
var mqttClient = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .Build();

await mqttClient.ConnectAsync(options);

var telemetry = new { DeviceId = "device123", Value = 25.5, Type = "temperature", Timestamp = DateTime.UtcNow };
var message = new MqttApplicationMessageBuilder()
    .WithTopic("iot/telemetry")
    .WithPayload(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(telemetry)))
    .Build();

await mqttClient.PublishAsync(message);
await mqttClient.DisconnectAsync();