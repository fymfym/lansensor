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
        public Configuration( string alternateConfigFilename)
        {
            string filename = @"polling.json";
            if (alternateConfigFilename?.Length > 0)
                filename = alternateConfigFilename;

            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                ApplicationConfiguration = (ApplicationConfiguration)serializer.Deserialize(file, typeof(ApplicationConfiguration));
            }
        }
    }
}
