using System.Configuration;

namespace LanSensor.Repository.DataStore.MySQL.Models
{
    public class Configuration
    {
        public string MySqlConnectionString;


        public Configuration()
        {
            MySqlConnectionString = ConfigurationManager.AppSettings.Get("mySqlConnectionString");
        }
    }
}
