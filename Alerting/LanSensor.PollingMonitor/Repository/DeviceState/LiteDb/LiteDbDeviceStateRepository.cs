
using System;
using System.Threading.Tasks;
using LanSensor.Models;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceState;

namespace LanSensor.Repository.DeviceState.LiteDb
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

        public async Task<DeviceStateEntity> GetLatestDeviceStateEntity(string deviceGroupId, string deviceId)
        {
            DeviceStateEntity result = null;

            string sql = "select DeviceGroipId,DeviceId,LastKnownDataValue,LastKnownDataValueDate,LastKnownKeepAlive,LastExecutedKeepaliveCheckDate " +
                         "from devicestate where DeviceGroupId = '" + deviceGroupId +"' and DeviceId = '" + deviceId + "'";
            string mySqlConnectionString = _configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString;
            using (var mysql = new MySql.Data.MySqlClient.MySqlConnection())
            {
                var strConnect = mySqlConnectionString;
                mysql.ConnectionString = strConnect;
                mysql.Open();

                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, mysql))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            result = new DeviceStateEntity()
                            {
                                DeviceGrouipId = rdr.GetString(0),
                                DeviceId = rdr.GetString(1),
                                LastKnownDataValue = rdr.GetString(2),
                                LastKnownDataValueDate = rdr.GetDateTime(3),
                                LastKnownKeepAlive = rdr.GetString(4),
                                LastExecutedKeepaliveCheckDate = rdr.GetDateTime(5)

                            };
                        }
                    }
                }
            }

            return result;
        }

        public Task<DeviceStateEntity> SetDeviceStateEntity(DeviceStateEntity deviceStateEntity)
        {
            // TODO Implement MySQL UPDATE
            return Task.Run(() => deviceStateEntity);
        }
    }
}
