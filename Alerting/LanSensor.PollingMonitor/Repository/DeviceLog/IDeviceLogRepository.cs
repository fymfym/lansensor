using System.Threading.Tasks;

namespace LanSensor.Repository.DeviceLog
{
    public interface IDeviceLogRepository
    {
        Task<Models.DeviceLog.DeviceLog> GetLatestPresence(string deviceGroupId, string deviceId);
        Task<Models.DeviceLog.DeviceLog> GetLatestPresence(string deviceGroupId, string deviceId,string dataType);
    }
}
