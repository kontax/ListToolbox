using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Authentication;
using BIApps.ListToolbox.Model.Helpers;

namespace BIApps.ListToolbox.Model.Users {

    /// <summary>
    /// This class represents the user logging into the application. It ensures they are part of the 
    /// correct Active Directory group in order to run the app, and checks the other groups they are 
    /// part of in order to use the Contact Checker.
    /// </summary>
    public class User {

        #region Properties & Fields

        /// <summary>
        /// The Active Directory group the user needs to be part of in order to use the application.
        /// </summary>
        private readonly string _applicationActiveDirectoryGroup;

        /// <summary>
        /// The list of Active Directory group that the user is currently part of already.
        /// </summary>
        private readonly List<string> _userActiveDirectoryGroups;

        private readonly List<string> _userValidActiveDirectoryGroups;
        /// <summary>
        /// A list of the Active Directory groups the user is part of that apply to this application.
        /// </summary>
        public List<string> ActiveDirectoryGroups {
            get { return _userValidActiveDirectoryGroups; }
        }

        /// <summary>
        /// The full login name of the user.
        /// </summary>
        public string Username { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="User"/> class and gets the required Active 
        /// Directory information from the database.
        /// </summary>
        public User() {

            // Establish user details
            Username = Environment.UserName;
            _userActiveDirectoryGroups = GetUserActiveDirectoryGroups();

            var config = new BiAppsDataContext();

            // Get the AD Group needed to use the plugin
            _applicationActiveDirectoryGroup = (from c in config.Config
                                               where c.application_name == "List Toolbox"
                                                     && c.category == "Permissions" && c.name == "ADGroup"
                                               select c.value).SingleOrDefault();

            // Make sure the user is part of the correct AD Group
            CheckPermissions();

            var validActiveDirectoryGroups = (from c in config.Config
                                              where c.application_name == "List Toolbox"
                                                    && c.category == "Channel"
                                              select c.name).Distinct().ToList();

            // Check the user is in an AD group which can use the contact checker
            _userValidActiveDirectoryGroups = GetRequiredGroups(validActiveDirectoryGroups);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Pulls back a list of Active Directory groups that the user is part of.
        /// </summary>
        /// <returns>A list of strings containing all the AD groups of the user</returns>
        private List<string> GetUserActiveDirectoryGroups() {
            var result = new List<string>();

            using(var pc = new PrincipalContext(ContextType.Domain, "PPOWER")) {
                UserPrincipal findByIdentity;
                using(findByIdentity = UserPrincipal.FindByIdentity(pc, Username)) {
                    if(findByIdentity != null)
                        using(var src = findByIdentity.GetGroups(pc)) {
                            src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                        }
                }
            }
            result.Sort();
            return result;
        }

        /// <summary>
        /// Checks to see whether the user is part of the correct group and if not throws an exception.
        /// </summary>
        private void CheckPermissions() {
            if (GetRequiredGroups(new List<string> {_applicationActiveDirectoryGroup}).Count == 0)
                throw new InvalidCredentialException(string.Format(
                    "You need to be in the \"{0}\" Active Directory group in order " +
                    "to use this application. Please fill out a request form requesting access.", 
                    _applicationActiveDirectoryGroup));
        }

        /// <summary>
        /// Compares the groups the user is part of to the ones required in the application.
        /// </summary>
        /// <param name="adGroups">The list of groups the user is part of</param>
        /// <returns>A list of those groups that are necessary for the application</returns>
        private List<string> GetRequiredGroups(IEnumerable<string> adGroups) {
            return (from u in adGroups
                    from g in _userActiveDirectoryGroups
                    where u.Contains(g)
                    select u).ToList();
        }

        #endregion
    }
}
