using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq;

namespace BIApps.ListToolbox.Model.Helpers {

    /// <summary>
    /// This class is a direct link to the BI Apps database. It connects using the initial connection string
    /// from the App.config file and is used by the Config class to pull back config details.
    /// </summary>
    [DatabaseAttribute(Name = "bi_apps")]
    public class BiAppsDataContext : DataContext {

        /// <summary>
        /// The bi_apps.config.application_config table storing configuration details.
        /// </summary>
        public Table<Config> Config;

        /// <summary>
        /// Creates a new instance of the BI Apps data context to get configuration details.
        /// </summary>
        public BiAppsDataContext()
            : base(StaticConfig.InitialConnectionString) {

            // Set the RO/RW connection strings from the DB
            StaticConfig.ReadOnlyConnectionString = (from c in Config
                                                     where c.application_name == "List Toolbox"
                                                     && c.category == "ConnectionString"
                                                     && c.name == "ReadOnly"
                                                     select c.value).SingleOrDefault();

            StaticConfig.ReadWriteConnectionString = (from c in Config
                                                      where c.application_name == "List Toolbox"
                                                      && c.category == "ConnectionString"
                                                      && c.name == "ReadOnly"
                                                      select c.value).SingleOrDefault();
        }
    }
}
