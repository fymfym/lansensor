using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanSensor.Models.Configuration;
using LanSensor.Models.DeviceLog;

namespace LanSensor.Repository.DeviceLog.MySqlDeviceLog
{
    public class MySqlDataStoreRepository : IDeviceLogRepository
    {
        private readonly IConfiguration _configuration;

        public MySqlDataStoreRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId)
        {
            return await GetLatestPresence(deviceGroupId, deviceId, null);
        }

        public async Task<DeviceLogEntity> GetLatestKeepalive(string deviceGroupId, string deviceId)
        {
            string sql = "select DataValue, DataType, DateTime from devicelog where ";

            sql += "DeviceGroupId = '" + deviceGroupId + "' " +
                   "and DeviceId='" + deviceId + "' " +
                   "and DataValue='keepalive' ";

            sql += "order by DateTime desc limit 1";

            var resultRecord = new DeviceLogEntity()
            {
                DeviceGroupId = deviceGroupId,
                DeviceId = deviceId
            };

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
                            resultRecord.DataValue = rdr.GetString(0);
                            resultRecord.DataType = rdr.GetString(1);
                            resultRecord.DateTime = rdr.GetDateTime(2);
                        }
                    }
                }
            }
            return resultRecord;
        }

        public async Task<DeviceLogEntity> GetLatestPresence(string deviceGroupId, string deviceId, string dataType)
        {
            string sql = "select DataValue, DataType, DateTime from DeviceLog where ";

            sql += "DeviceGroupId = '" + deviceGroupId + "' " + 
                   "and DeviceId='" + deviceId + "' ";

            if (!string.IsNullOrEmpty(dataType))
                sql += "and DataType='" + dataType + "' ";

            sql += "order by DateTime desc limit 1";

            DeviceLogEntity resultRecord = null;

            string mySqlConnectionString = _configuration.ApplicationConfiguration.MySqlConfiguration.ConnectionString;
            using (var mysql = new MySql.Data.MySqlClient.MySqlConnection())
            {
                mysql.ConnectionString = mySqlConnectionString;
                mysql.Open();

                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, mysql))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            resultRecord = new DeviceLogEntity()
                            {
                                DeviceGroupId = deviceGroupId,
                                DeviceId = deviceId,
                                DataValue = rdr.GetString(0),
                                DataType = rdr.GetString(1),
                                DateTime = rdr.GetDateTime(2)
                            };
                        }
                    }
                }
            }
            return resultRecord;
        }

        public Task<IEnumerable<DeviceLogEntity>> GetPresenceListSince(string deviceGroupId, string deviceId, DateTime lastKnownPresence)
        {
            throw new NotImplementedException();
        }
    }
}
