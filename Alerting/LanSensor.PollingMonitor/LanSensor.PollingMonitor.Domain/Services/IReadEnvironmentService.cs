namespace LanSensor.PollingMonitor.Domain.Services
{
    public interface IReadEnvironmentService
    {
        string GetEnvironmentVariable(string environmentName);
    }
}
