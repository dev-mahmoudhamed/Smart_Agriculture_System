namespace Smart_Agriculture_System.BackgroundServices
{
    public interface ISensorDataJob
    {
        Task LoadSensorDataAsync();
        Task LoadImageDataAsync();
    }
}
