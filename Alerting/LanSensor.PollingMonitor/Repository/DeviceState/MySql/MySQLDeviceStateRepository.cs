using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceState;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace LanSensor.Repository.DeviceState.MySql
{
    public class MySqlDeviceStateRepository : IDeviceStateRepository
    {
        private readonly IConfiguration _configuration;

        public MySqlDeviceStateRepository
        (
            IConfiguration configuration
        )
        {
            _configuration = configuration;
        }

        public Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId)
        {
            DeviceStateEntity result;
            string sql = "select DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAliveDate, LastExecutedKeepaliveCheckDate, LastKeepAliveAlert ";
            sql += " from devicestate where ";

            sql += "DeviceGroupId = '" + deviceGroupId + "' " +
                   "and DeviceId='" + deviceId + "' ";

            using (var mysql = new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
            {
                result = mysql.Query<DeviceStateEntity>(sql).FirstOrDefault();
            }

            return Task.Run(() => result);
        }

        public async Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            DeviceStateEntity result = await GetLatestDeviceStateEntity(deviceStateEntity.DeviceGroupId, deviceStateEntity.DeviceId);
            if (result == null)
            {
                await InsertDeviceStateEntity(deviceStateEntity);
            }
            else
            {
                await UpdateDeviceStateEntity(deviceStateEntity);
            }

            return deviceStateEntity;
        }

        private async Task<DeviceStateEntity> UpdateDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            string sql = "update devicestate set LastKnownDataValue=@LastKnownDataValue, LastKnownDataValueDate=@LastKnownDataValueDate, LastKnownKeepAlive=@LastKnownKeepAlive, LastExecutedKeepaliveCheckDate=@LastExecutedKeepaliveCheckDate,LastKeepAliveAlert=@LastKeepAliveAlert ";
            sql += "where ";

            sql += "DeviceGroupId = '" + deviceStateEntity.DeviceGroupId + "' " +
                   "and DeviceId='" + deviceStateEntity.DeviceId + "' ";

            using (var mysql = new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
            {
                await mysql.ExecuteAsync(sql, deviceStateEntity);
            }

            return deviceStateEntity;
        }

        private async Task<DeviceStateEntity> InsertDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            string sql = "insert devicestate into (DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAlive, LastExecutedKeepaliveCheckDate,LastKeepAliveAlert) value (@DeviceGroupId, @DeviceId, @LastKnownDataValue, @LastKnownDataValueDate, @LastKnownKeepAlive, @LastExecutedKeepaliveCheckDate, @LastKeepAliveAlert) ";
            sql += "where ";

            sql += "DeviceGroupId = '" + deviceStateEntity.DeviceGroupId + "' " +
                   "and DeviceId='" + deviceStateEntity.DeviceId + "' ";

            using (var mysql = new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
            {
                await mysql.ExecuteAsync(sql, deviceStateEntity);
            }

            return deviceStateEntity;
        }
    }
}
