using System;
using System.IO;
using LanSensor.PollingMonitor.Domain.Repositories;
using LanSensor.PollingMonitor.Domain.Services;
using Newtonsoft.Json;

namespace LanSensor.PollingMonitor.Domain.Models
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        private readonly IReadEnvironmentService _readEnvironmentService;
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        public ServiceConfiguration(
            IReadEnvironmentService readEnvironmentService
            )
        {
            _readEnvironmentService = readEnvironmentService;
            Init(null);
        }

        /// <summary>
        /// Reads "polling.json" is no parameter is given
        /// </summary>
        /// <param name="readEnvironmentService"></param>
        /// <param name="alternateConfigFilename"></param>
        public ServiceConfiguration(
            IReadEnvironmentService readEnvironmentService,
            string alternateConfigFilename)
        {
            _readEnvironmentService = readEnvironmentService;
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

            if (ApplicationConfiguration == null)
            {
                throw new Exception("Configuration deserialize from 'polling(_*).json' fails");
            }

            ApplicationConfiguration.MongoDbConfiguration = new MongoDbConfiguration
            {
                ConnectionString = Environment.GetEnvironmentVariable("mongoDbConnectionString")
            };

            var ur = _readEnvironmentService.GetEnvironmentVariable("slackApiUrl");
            ApplicationConfiguration.SlackConfiguration = new SlackConfiguration
            {
                ApiKey = _readEnvironmentService.GetEnvironmentVariable("slackApiKey"),
                ChannelId = _readEnvironmentService.GetEnvironmentVariable("slackChannelId"),
                ApiUrl = new Uri(_readEnvironmentService.GetEnvironmentVariable("slackApiUrl") ?? "")
            };

            ApplicationConfiguration.RestServiceConfiguration = new RestServiceConfiguration
            {
                DeviceRestApiBasePath = _readEnvironmentService.GetEnvironmentVariable("deviceRestApiBasePath")
            };

            ApplicationConfiguration.MySqlConfiguration = new MySqlConfiguration
            {
                ConnectionString = _readEnvironmentService.GetEnvironmentVariable("mySqlConnectionString")
            };
        }
    }
}
