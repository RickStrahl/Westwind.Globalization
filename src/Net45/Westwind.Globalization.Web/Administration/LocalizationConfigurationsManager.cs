using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Westwind.Utilities;

namespace Westwind.Globalization.Web.Administration
{
    /// <summary>
    /// This class manages saving configuration settings so they can be easily restored
    /// in LocalizationsConfigurations.json in the Web root folder
    /// </summary>
    /// <remarks>
    /// Assumes Web application has rights to read/write to disk in the Web root in
    /// order to save settings.
    /// </remarks>
    public class LocalizationConfigurationsManager
    {
        public List<ConfigurationEntry> Configurations = new List<ConfigurationEntry>();

        public bool Load(string filename = "~/LocalizationConfigurations.json")
        {
            if (filename.StartsWith("~/"))
                filename = HttpContext.Current.Server.MapPath(filename);

            if (!File.Exists(filename))
                return false;

            var config =
                JsonSerializationUtils.DeserializeFromFile(filename, typeof (ConfigurationEntry))
                    as List<ConfigurationEntry>;

            return config != null;
        }

        public bool Save(string filename = "~/LocalizationConfigurations.json")
        {
            if (filename.StartsWith("~/"))
                filename = HttpContext.Current.Server.MapPath(filename);


            return JsonSerializationUtils.SerializeToFile(Configurations, filename, false, true);
        }

        /// <summary>
        /// Saves the current configuration settings
        /// </summary>
        /// <param name="name"></param>
        public void StoreConfiguration(string name, DbResourceConfiguration config = null)
        {
            if (config == null)
                config = DbResourceConfiguration.Current;

            var existingItem = Configurations.Where(c => c.Name == name).FirstOrDefault();
            if (existingItem == null)
            {
                var configuration = new ConfigurationEntry()
                {
                    Name = name
                };

                configuration.Configuration = new DbResourceConfiguration();
                DataUtils.CopyObjectData(config, configuration.Configuration);
            }
            else
            {
                DataUtils.CopyObjectData(config, existingItem.Configuration);
            }
        }

        public bool SetConfiguration(string name)
        {
            var existingItem = Configurations.Where(c => c.Name == name).FirstOrDefault();
            if (existingItem == null)
                return false;

            var config = DbResourceConfiguration.Current;
            DataUtils.CopyObjectData(existingItem.Configuration, DbResourceConfiguration.Current);

            return true;
        }

        public void RemoveConfiguration(string name)
        {
            var existingItem = Configurations.Where(c => c.Name == name).FirstOrDefault();
            if (existingItem != null)
                Configurations.Remove(existingItem);
        }

    }

    public class ConfigurationEntry
    {
        public string Name { get; set; }
        public DbResourceConfiguration Configuration { get; set; }
    }
}
