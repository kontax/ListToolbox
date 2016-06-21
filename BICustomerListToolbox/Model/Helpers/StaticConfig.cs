using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.Model.Helpers {

    /// <summary>
    /// Contains the static configuration details used throughout the application
    /// </summary>
    public static class StaticConfig {

        public static string InitialConnectionString =
            "Data Source=SQLRA_DB_BI_APPS;Persist Security Info=False;Integrated Security=SSPI;Initial Catalog=bi_apps;";
        public static string ReadOnlyConnectionString;
        public static string ReadWriteConnectionString;
    }
}
