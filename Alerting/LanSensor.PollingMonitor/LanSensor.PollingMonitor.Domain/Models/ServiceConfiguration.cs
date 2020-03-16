using System;
using System.IO;
using LanSensor.PollingMonitor.Domain.Repositories;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        public ServiceConfiguration()
        {
            Init(null);
        }

        /// <summary>
        /// Reads "polling.json" is no parameter is given
        /// </summary>
        /// <param name="alternateConfigFilename"></param>
        public ServiceConfiguration(string alternateConfigFilename)
        {
            Init(alternateConfigFilename);
        }

        private void Init(string alternateConfigFilename)
        {
            string filename;
            if (alternateConfigFilename?.Length > 0)
            {
                filename = alternateConfigFilename;
            }
            else
            {
                var altFile = new FileInfo("polling_temp.json");
                if (!altFile.Exists)
                {
                    altFile = new FileInfo("polling_develop.json");
                }

                if (!altFile.Exists)
                {
                    altFile = new FileInfo("polling_develop.json");
                }

                if (!altFile.Exists)
                {
                    altFile = new FileInfo("polling.json");
                }

                filename = altFile.Name;
            }

            using (var file = File.OpenText(filename))
            {
                var serializer = new JsonSerializer();
                ApplicationConfiguration =
                    (ApplicationConfiguration) serializer.Deserialize(file, typeof(ApplicationConfiguration));
            }

            ApplicationConfiguration.MongoConfiguration = new MongoConfiguration()
            {
                ConnectionString = Environment.GetEnvironmentVariable("mongodbconnectionstring")
            };

            ApplicationConfiguration.SlackConfiguration = new SlackConfiguration()
            {
                ApiKey = Environment.GetEnvironmentVariable("slackapikey")
            };

            ApplicationConfiguration.RestServiceConfiguration = new RestServiceConfiguration()
            {
                DeviceRestApiBasePath = Environment.GetEnvironmentVariable("deviceRestApiBasePath")
            };

            ApplicationConfiguration.MySqlConfiguration = new MySqlConfiguration()
            {
                ConnectionString = Environment.GetEnvironmentVariable("MySqlConnectionString")
            };
        }
    }
}
