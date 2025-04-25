using Smart_Agriculture_System.Models;

namespace Smart_Agriculture_System.Services
{
    public interface ISensorDataServices
    {
        Task<SensorReading> GetAllSensorDataAsync();
    }
}
