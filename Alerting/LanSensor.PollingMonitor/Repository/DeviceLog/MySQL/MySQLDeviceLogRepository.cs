using System.Threading.Tasks;
using LanSensor.Models.Configuration;

namespace LanSensor.Repository.DeviceLog.MySQL
{
    public class MySqlDataStoreRepository : IDeviceLogRepository
    {
        private readonly IConfiguration _confiugration;

        public MySqlDataStoreRepository(IConfiguration configuration)
        {
            _confiugration = configuration;
        }

        public async Task<Models.DeviceLog.DeviceLog> GetLatestPresence(string deviceGroupId, string deviceId)
        {
            return await GetLatestPresence(deviceGroupId, deviceId, null);
        }

        public async Task<Models.DeviceLog.DeviceLog> GetLatestPresence(string deviceGroupId, string deviceId, string dataType)
        {
            string sql = "select DataValue, DataType, DateTime from sensorstate where ";

            sql += "DeviceGroupId = '" + deviceGroupId + 
                   "' and DeviceId='" + deviceId + "' ";

            if (!string.IsNullOrEmpty(dataType))
                sql = "and DataType='" + dataType + "''";

            sql += "order by DateTime desc limit 1";

            Models.DeviceLog.DeviceLog resultRecord = new Models.DeviceLog.DeviceLog()
            {
                DeviceGroupId = deviceGroupId,
                DeviceId = deviceId
            };

            string mySqlConnectionString = _confiugration.ApplicationConfiguration.MySqlConfiguration.ConnectionString;
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
    }
}
