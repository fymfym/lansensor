using System;
using LanSensor.Repository.DataStore.MySQL.Models;

namespace LanSensor.Repository.DataStore.MySQL
{
    public class MySqlDataStoreRepository : IDataStoreRepository
    {
        private readonly Configuration _configruation;

        public MySqlDataStoreRepository(Configuration configruation)
        {
            _configruation = configruation;
        }

        public DateTime GetLatestPresence(string dataType)
        {
            return GetPrecense(dataType, null);
        }

        public DateTime GetLatestPresence(string dataType, string dataValue)
        {
            return GetPrecense(dataType, dataValue);
        }

        private DateTime GetPrecense(string dataType, string dataValue)
        {
            DateTime res = DateTime.MinValue;
            string sql;
            if (string.IsNullOrEmpty(dataValue))
            {
                sql = "select DateTime from sensorstate where DataType = '" + dataType + "'  order by DateTime desc limit 1";
            }
            else
            {
                sql = "select DateTime from sensorstate where DataType = '" + dataType + "' and DataValue = '" + dataValue + "' order by DateTime desc limit 1";
            }

            string mySqlConnectionString = _configruation.MySqlConnectionString;
            using (var mysql = new MySql.Data.MySqlClient.MySqlConnection())
            {
                var strConnect = mySqlConnectionString;
                mysql.ConnectionString = strConnect;
                mysql.Open();

                using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, mysql))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            res = rdr.GetDateTime(0);
                        }
                    }
                }
            }
            return res;

        }

    }
}
