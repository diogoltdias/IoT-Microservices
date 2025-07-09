using DeviceRegistryService.Models;
using MongoDB.Driver;

namespace DeviceRegistryService.Services;
public class DeviceService
{
    private readonly IMongoCollection<Device> _devices;

    public DeviceService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDb:ConnectionString"]);
        var database = client.GetDatabase(config["MongoDb:DatabaseName"]);
        _devices = database.GetCollection<Device>("Devices");
    }

    public async Task<List<Device>> GetAllAsync() =>
        await _devices.Find(_ => true).ToListAsync();

    public async Task<Device?> GetByIdAsync(string id) =>
        await _devices.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task<Device> CreateAsync(Device device)
    {
        await _devices.InsertOneAsync(device);
        return device;
    }

    public async Task UpdateAsync(string id, Device device) =>
        await _devices.ReplaceOneAsync(d => d.Id == id, device);

    public async Task DeleteAsync(string id) =>
        await _devices.DeleteOneAsync(d => d.Id == id);
}