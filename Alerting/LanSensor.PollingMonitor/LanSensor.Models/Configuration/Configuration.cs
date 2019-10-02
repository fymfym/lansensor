using System.IO;
using Newtonsoft.Json;

namespace LanSensor.Models.Configuration
{
    public class Configuration : IConfiguration
    {
        public ApplicationConfiguration ApplicationConfiguration { get; set; }


        /// <summary>
        /// Reads "polling.json" is no parameter is given
        /// </summary>
        /// <param name="alternateConfigFilename"></param>
        public Configuration(string alternateConfigFilename)
        {
            string filename;
            if (alternateConfigFilename?.Length > 0)
            {
                filename = alternateConfigFilename;
            }
            else
            {
                var altFile = new FileInfo("polling_production.json");
                if (!altFile.Exists)
                    altFile = new FileInfo("polling_develop.json");
                if (!altFile.Exists)
                    altFile = new FileInfo("polling.json");
                filename = altFile.Name;
            }

            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                ApplicationConfiguration =
                    (ApplicationConfiguration) serializer.Deserialize(file, typeof(ApplicationConfiguration));
            }
        }
    }
}
