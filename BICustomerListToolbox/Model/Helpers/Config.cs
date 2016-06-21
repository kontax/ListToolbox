using System.Data.Linq.Mapping;

namespace BIApps.ListToolbox.Model.Helpers {

    /// <summary>
    /// This class pulls back configuration details from the BI Apps database.
    /// </summary>
    [TableAttribute(Name = "config.application_config")]
    public class Config {

        private readonly int _application_config_id;
        private readonly string _application_name;
        private readonly string _category;
        private readonly string _name;
        private readonly string _value;
        private readonly string _description;

        [ColumnAttribute(Storage = "_application_config_id", DbType = "Int NOT NULL")]
        public int application_config_id {
            get { return _application_config_id; }
        }

        [ColumnAttribute(Storage = "_application_name", DbType = "VarChar(255)")]
        public string application_name {
            get { return _application_name; }
        }

        [ColumnAttribute(Storage = "_category", DbType = "VarChar(255)")]
        public string category {
            get { return _category; }
        }

        [ColumnAttribute(Storage = "_name", DbType = "VarChar(255)")]
        public string name {
            get { return _name; }
        }

        [ColumnAttribute(Storage = "_value", DbType = "VarChar(255)")]
        public string value {
            get { return _value; }
        }

        [ColumnAttribute(Storage = "_description", DbType = "VarChar(250)")]
        public string description {
            get { return _description; }
        }
    }
}
