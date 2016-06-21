using System.Collections.Generic;
using System.Linq;
using BIApps.ListToolbox.Model.Users;

namespace BIApps.ListToolbox.Model.Helpers {

    /// <summary>
    /// Contains all the business channels a user can check a list for contactability from.
    /// </summary>
    public class BusinessChannelValues {

        /// <summary>
        /// The list of channels available to the user.
        /// </summary>
        public List<string> Channels { get; set; }

        /// <summary>
        /// This populates the list of channels with only those channels that are available to the user.
        /// </summary>
        public BusinessChannelValues() {
            var user = new User();
            var config = new BiAppsDataContext();
            Channels = (from c in config.Config
                        where c.application_name == "List Toolbox"
                              && user.ActiveDirectoryGroups.Contains(c.name)
                              orderby c.application_config_id
                        select c.value).Distinct().ToList();
        }
    }
}
