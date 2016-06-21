using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;


namespace BIApps.ListToolbox {

    public class SqlParameterData {
        public string Name;
        public SqlDbType Type;
        public object Value;

        public SqlParameterData(string name, SqlDbType type, object value) {
            Name = name;
            Type = type;
            Value = value;
        }
    }

    public enum ConnectionType {
        ConnectionStringReadOnly,
        ConnectionStringReadWrite
    }


    class DBCommon {

        #region Properties

        /// <summary>
        /// The hard coded connection used to pull back the connection strings from the config table
        /// TODO: Replace this with the new BI Apps DNS when available
        /// </summary>
        private static string _initialDatabaseConnectionString = "Trusted_Connection=Yes;Data Source=SQLRA_DB;Persist Security Info=False;Initial Catalog=bi_apps;";

        /// <summary>
        /// The stored procedure which contains the Readonly & Read/Write connection strings
        /// </summary>
        private static string _domainValuesProcName = "bi_apps.config.get_application_domain_values";

        /// <summary>
        /// The name of the application to pass to the proc containing the connection strings
        /// </summary>
        private static string _departmentName = "List Toolbox";

        #endregion

        #region Private methods

        /// <summary>
        /// Execute a stored procedure against the warehouse, and pull back a DataTable with the results.
        /// </summary>
        /// <param name="connection">The connection to use against the DB</param>
        /// <param name="storedProcName">The name of the stored proc to query</param>
        /// <param name="procParams">The list of parameters to send to the stored proc</param>
        /// <returns>A DataTable with the results of the stored proc</returns>
        private static DataTable ExecuteStoredProcedure(SqlConnection connection,
                                                        string storedProcName,
                                                        List<SqlParameterData> procParams) {

            // Sort out the query details
            SqlCommand queryCommand = new SqlCommand(storedProcName, connection);
            queryCommand.CommandType = CommandType.StoredProcedure;
            queryCommand.CommandTimeout = 0;

            // Add the parameters to the query
            foreach(SqlParameterData spd in procParams) {
                if(spd.Type == SqlDbType.Structured) {
                    queryCommand.Parameters.AddWithValue(spd.Name, spd.Value).SqlDbType = SqlDbType.Structured;
                } else {
                    queryCommand.Parameters.Add(new SqlParameter(spd.Name, spd.Type)).Value = spd.Value;
                }
            }

            // Execute the proc and add it to a DataTable
            SqlDataReader queryCommandReader = queryCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(queryCommandReader);

            return dataTable;
        }

        /// <summary>
        /// Get the connection string from the config table in the DB.
        /// </summary>
        /// <param name="connType">Whether we need a Read Only or Read/Write connection</param>
        /// <returns>A string containing the new connection details</returns>
        private static string GetConnectionString(ConnectionType connType) {
            //_sdwDBConnection = new SqlConnection(_initialDatabaseConnectionString);
            //_sdwDBConnection.Open();

            using(SqlConnection initConnection = new SqlConnection(_initialDatabaseConnectionString)) {

                initConnection.Open();

                // Parameters passed to the proc containing the connection info
                var connectionParams = new List<SqlParameterData>() {
                    new SqlParameterData("@QUERY_ID", SqlDbType.Int, 0),
                    new SqlParameterData("@department", SqlDbType.Text, _departmentName),
                    new SqlParameterData("@param_name", SqlDbType.Text, connType)
                };

                return ExecuteStoredProcedure(initConnection, _domainValuesProcName, connectionParams).Rows[0][0].ToString();
            }
        }

        /// <summary>
        /// Open either a ReadOnly or Read/Write connection to the DWH 
        /// </summary>
        /// <param name="connType">The ConnectionType to connect to the DB with</param>
        /// <returns>An SqlConnection to connect to the DB with</returns>
        private static SqlConnection OpenDBConnection(ConnectionType connType) {
            string connectionString = GetConnectionString(connType);
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Get the AD Groups a user is part of
        /// </summary>
        /// <param name="userName">The login name of the user</param>
        /// <returns>A List of strings of all the AD groups a user is part of</returns>
        private static List<string> GetActiveDirectoryGroups(string userName) {
            List<string> result = new List<string>();

            using(PrincipalContext pc = new PrincipalContext(ContextType.Domain, "PPOWER")) {
                using(PrincipalSearchResult<Principal> src = UserPrincipal.FindByIdentity(pc, userName).GetGroups(pc)) {
                    src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                }
            }
            result.Sort();
            return result;
        }

        #endregion

        #region Query methods

        /// <summary>
        /// Execute a stored proc with a list of paramters.
        /// </summary>
        /// <param name="sproc_name">Name of the stored procedure that should be called</param>
        /// <param name="sproc_params">List of the stored procedure attributes </param>
        /// <returns>A CustomerList with the results from the query</returns>
        public static CustomerList ExecuteStoredProcedure(string sprocName, List<SqlParameterData> sprocParams, ConnectionType connType) {
            CustomerList output = new CustomerList();

            using(SqlConnection dbConnection = OpenDBConnection(connType)) {
                dbConnection.Open();
                DataTable dataTable = ExecuteStoredProcedure(dbConnection, sprocName, sprocParams);
                output = CustomerList.DataTableToCustomerList(dataTable);

                return output;
            }
        }

        /// <summary>
        /// Method which gets the views that a user has access to in dwarf
        /// </summary>
        /// <returns>
        /// A DataTable with the following columns:
        ///     schema_name (as in dwarf)
        ///     schema_viewed_name (as in the program)
        /// </returns>
        public static DataTable getUserSchemas() {
            //DataTable schemas = new DataTable();

            // Get the AD Groups the user is part of to cross reference it to the schema's 
            // they need to use
            List<string> adGroups = GetActiveDirectoryGroups(Environment.UserName);
            string adGroupsString = string.Join(",", adGroups.AsEnumerable().ToArray());

            // Parameters to check the proc against
            var connectionParams = new List<SqlParameterData>() {
                new SqlParameterData("@QUERY_ID", SqlDbType.Int, 1),
                new SqlParameterData("@department", SqlDbType.Text, _departmentName),
                new SqlParameterData("@param_name", SqlDbType.Text, adGroupsString)
            };

            // Schemas available to the user
            DataTable schemas = ExecuteStoredProcedure(_domainValuesProcName, connectionParams, ConnectionType.ConnectionStringReadOnly);

            // Everyone has access to the pp_online schema
            DataRow dr = schemas.NewRow();
            schemas.Rows.InsertAt(dr, schemas.Rows.Count);
            schemas.Rows[schemas.Rows.Count - 1][0] = "PP.com";

            return schemas.AsEnumerable().Distinct().CopyToDataTable();
        }


        /// <summary>
        /// Get a list of the columns that users can join to the DB from using a customer list.
        /// </summary>
        /// <returns>A datatable containing the different columns to join on</returns>
        public static DataTable GetCustomerJoinColumns() {

            // Pull the list of CustomerJoinColumns from the domain values table
            List<SqlParameterData> tabParams = new List<SqlParameterData>();
            string tabProcName = _domainValuesProcName;
            tabParams.Add(new SqlParameterData("@QUERY_ID", SqlDbType.Int, 0));
            tabParams.Add(new SqlParameterData("@department", SqlDbType.NText, "Generic"));
            tabParams.Add(new SqlParameterData("@param_name", SqlDbType.NText, "CustomerJoinColumn"));
            return ExecuteStoredProcedure(tabProcName, tabParams, ConnectionType.ConnectionStringReadOnly);
        }

        #endregion
    }
}
