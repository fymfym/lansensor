using System;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceState;
using System.Linq;
using Dapper;
using NLog;
using MySql.Data.MySqlClient;

namespace LanSensor.Repository.DeviceState.MySql
{
    public class MySqlDeviceStateRepository : IDeviceStateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public MySqlDeviceStateRepository
        (
            IConfiguration configuration,
            ILogger logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId)
        {
            DeviceStateEntity result;
            string sql = "select DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAliveDate, LastExecutedKeepAliveCheckDate, LastKeepAliveAlert ";
            sql += " from devicestate where ";

            sql += "DeviceGroupId = '" + deviceGroupId + "' " +
                   "and DeviceId='" + deviceId + "' ";

            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    result = mysql.Query<DeviceStateEntity>(sql).FirstOrDefault();
                }

                return Task.Run(() => result);
            }
            catch (Exception ex)
            {
                _logger.Fatal($"InsertDeviceStateEntity Execution failes: {ex.Message}");
            }

            return null;
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
            string sql = "update devicestate set LastKnownDataValue=@LastKnownDataValue, LastKnownDataValueDate=@LastKnownDataValueDate, LastKnownKeepAliveDate=@LastKnownKeepAlive, LastExecutedKeepAliveCheckDate=@LastExecutedKeepAliveCheckDate,LastKeepAliveAlert=@LastKeepAliveAlert ";
            sql += "where ";

            sql += "DeviceGroupId = '" + deviceStateEntity.DeviceGroupId + "' " +
                   "and DeviceId='" + deviceStateEntity.DeviceId + "' ";

            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    await mysql.ExecuteAsync(sql, deviceStateEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"UpdateDeviceStateEntity Execution failes: {ex.Message}");
            }

            return deviceStateEntity;
        }

        private Task<DeviceStateEntity> InsertDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            string sql = "insert into devicestate (DeviceGroupId, DeviceId, LastKnownDataValue, LastKnownDataValueDate, LastKnownKeepAliveDate, LastExecutedKeepAliveCheckDate,LastKeepAliveAlert) value (@DeviceGroupId, @DeviceId, @LastKnownDataValue, @LastKnownDataValueDate, @LastKnownKeepAlive, @LastExecutedKeepAliveCheckDate, @LastKeepAliveAlert) ";
            try
            {
                using (var mysql =
                    new MySqlConnection(_configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString))
                {
                    mysql.Open();
                    var cmd = new MySqlCommand {CommandText = sql, Connection = mysql};
                    cmd.Parameters.AddWithValue("DeviceGroupId", deviceStateEntity.DeviceGroupId);
                    cmd.Parameters.AddWithValue("DeviceId", deviceStateEntity.DeviceId);
                    cmd.Parameters.AddWithValue("LastKnownDataValue", deviceStateEntity.LastKnownDataValue);
                    cmd.Parameters.AddWithValue("LastKnownDataValueDate", deviceStateEntity.LastKnownDataValueDate);
                    cmd.Parameters.AddWithValue("LastKnownKeepAlive", deviceStateEntity.LastKnownKeepAlive);
                    cmd.Parameters.AddWithValue("LastExecutedKeepAliveCheckDate", deviceStateEntity.LastExecutedKeepAliveCheckDate);
                    cmd.Parameters.AddWithValue("LastKeepAliveAlert", deviceStateEntity.LastKeepAliveAlert);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal($"InsertDeviceStateEntity Execution failes: {ex.Message}");
            }

            return Task.FromResult(deviceStateEntity);
        }
    }
}
