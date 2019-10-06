using Newtonsoft.Json;
using System.IO;

namespace LanSensor.Models.Configuration
{
    public class Configuration : IConfiguration
    {
        public ApplicationConfiguration ApplicationConfiguration { get; set; }

        public Configuration()
        {
            Init(null);
        }

        /// <summary>
        /// Reads "polling.json" is no parameter is given
        /// </summary>
        /// <param name="alternateConfigFilename"></param>
        public Configuration(string alternateConfigFilename)
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
        }
    }
}
