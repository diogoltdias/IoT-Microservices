using DeviceRegistryService.Models;
using DeviceRegistryService.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace DeviceRegistryService.Tests;
public class DeviceServiceTests
{
    private readonly Mock<IMongoCollection<Device>> _mockCollection;
    private readonly DeviceService _service;

    public DeviceServiceTests()
    {
        _mockCollection = new Mock<IMongoCollection<Device>>();
        var mockDb = new Mock<IMongoDatabase>();
        mockDb.Setup(db => db.GetCollection<Device>(It.IsAny<string>())).Returns(_mockCollection.Object);
        var mockClient = new Mock<IMongoClient>();
        mockClient.Setup(c => c.GetDatabase(It.IsAny<string>())).Returns(mockDb.Object);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new[] { KeyValuePair.Create("MongoDb:ConnectionString", "mongodb://test"), KeyValuePair.Create("MongoDb:DatabaseName", "test") })
            .Build();

        _service = new DeviceService(config);
    }

    [Fact]
    public Task CreateAsyncDevice()
    {
        var device = new Device { Name = "TestDevice", Type = "Sensor" };
        _mockCollection.Setup(c => c.InsertOneAsync(It.IsAny<Device>(), nullIt.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(device);

        Assert.Equal(device, result);
        _mockCollection.Verify(c => c.InsertOneAsync(It.IsAny<Device>())).Timescale.Once());
        return result;
    }
}