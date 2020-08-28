namespace LanSensor.PollingMonitor.Infrastructure.RestServices
{
    public interface IMessageSender
    {
        bool SendMessage(string message);
    }
}
