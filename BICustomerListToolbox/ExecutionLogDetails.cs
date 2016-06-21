using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApps.ListToolbox {
    class ExecutionLogDetails {

        #region Private Fields

        /// <summary>
        /// Stored procedure used to log the usage of the application
        /// </summary>
        private string _log_proc = "bi_apps.list_toolbox.list_toolbox_execution_log";

        /// <summary>
        /// Name of the application being logged
        /// </summary>
        private string _application = "BI Customer List Toolbox";

        /// <summary>
        /// Version of the application being logged
        /// </summary>
        private string _app_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Login name of the user using the application
        /// </summary>
        private string _user_id = Convert.ToString(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

        /// <summary>
        /// PC name of the user using the application
        /// </summary>
        private string _pc_name = System.Environment.MachineName;

        /// <summary>
        /// Section within the application being used
        /// </summary>
        private string _section;

        /// <summary>
        /// Subsection within the application being used
        /// </summary>
        private string _sub_section;

        /// <summary>
        /// Datetime the section/sub-section execution started
        /// </summary>
        private DateTime _start_exec_datetime;

        /// <summary>
        /// Datetime the section/sub-section execution finished
        /// </summary>
        private DateTime _end_exec_datetime;

        /// <summary>
        /// The number of CustomerLists inputted prior to execution
        /// </summary>
        private Int32 _input_listcount;

        /// <summary>
        /// The number of CustomerLists outputted after execution
        /// </summary>
        private Int32 _output_listcount;

        /// <summary>
        /// The total number of rows for all CustomerLists prior to execution
        /// </summary>
        private Int32 _input_rowcount;

        /// <summary>
        /// The total number of rows for all CustomerLists after execution
        /// </summary>
        private Int32 _output_rowcount;

        /// <summary>
        /// Any exceptions that were caught and logged
        /// </summary>
        private StringBuilder _error_messages = new StringBuilder();


        #endregion

        #region Initializers

        /// <summary>
        /// Create a new log for uploading to the database
        /// </summary>
        public ExecutionLogDetails() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enter the parameters used before execution takes place.
        /// </summary>
        /// <param name="section">The section of the application being used</param>
        /// <param name="subsection">The subsection of the application being used</param>
        /// <param name="input">The CustomerListSet containing the lists being worked on</param>
        public void LogStart(string section, CustomerListSet input) {
            LogClear();
            this._section = section;
            this._start_exec_datetime = DateTime.Now;
            this._input_listcount = input.Tables.Count;
            foreach(CustomerList cl in input.Tables) {
                this._input_rowcount += cl.Rows.Count;
            }
        }

        /// <summary>
        /// Enter the parameters used after execution takes place, and send to the DB
        /// </summary>
        /// <param name="output">The CustomerListSet containing the outputted lists</param>
        public void LogEnd(string subsection, CustomerListSet output) {
            this._sub_section = subsection;
            this._end_exec_datetime = DateTime.Now;
            this._output_listcount = output.Tables.Count;
            foreach(CustomerList cl in output.Tables) {
                this._output_rowcount += cl.Rows.Count;
            }
            FinalizeErrors();
            LogToDB();
        }

        /// <summary>
        /// Append any errors occurring to the log.
        /// </summary>
        /// <param name="errorMessage">The exception thrown</param>
        public void LogError(string errorMessage) {
            this._error_messages.Append("<error>" + errorMessage + "</error>\n");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add the final XML tags to the error messages
        /// </summary>
        private void FinalizeErrors() {
            this._error_messages.Insert(0, "<errors>\n");
            this._error_messages.Append("</errors>");
        }

        /// <summary>
        /// Clear all entries in the log
        /// </summary>
        private void LogClear() {
            this._section = null;
            this._sub_section = null;
            this._start_exec_datetime = DateTime.MinValue;
            this._input_listcount = 0;
            this._input_rowcount = 0;
            this._end_exec_datetime = DateTime.MinValue;
            this._output_listcount = 0;
            this._output_rowcount = 0;
            this._error_messages.Clear();
        }

        /// <summary>
        /// Send the log to the Database
        /// </summary>
        private void LogToDB() {
            var execLogParams = this.ToSqlParameterList();
            //DBCommon.ExecuteStoredProcedure(_log_proc, execLogParams, ConnectionType.ConnectionStringReadWrite);
        }


        /// <summary>
        /// Convert the log details to an SqlParameterList
        /// </summary>
        /// <returns>An SqlParameterList of the required parameters for logging</returns>
        private List<SqlParameterData> ToSqlParameterList() {
            List<SqlParameterData> output = new List<SqlParameterData>();

            output.Add(new SqlParameterData("@application", SqlDbType.Text, this._application));
            output.Add(new SqlParameterData("@app_version", SqlDbType.Text, this._app_version));
            output.Add(new SqlParameterData("@user_id", SqlDbType.Text, this._user_id));
            output.Add(new SqlParameterData("@pc_name", SqlDbType.Text, this._pc_name));
            output.Add(new SqlParameterData("@section", SqlDbType.Text, this._section));
            output.Add(new SqlParameterData("@sub_section", SqlDbType.Text, this._sub_section));
            output.Add(new SqlParameterData("@start_exec_datetime", SqlDbType.DateTime, this._start_exec_datetime));
            output.Add(new SqlParameterData("@end_exec_datetime", SqlDbType.DateTime, this._end_exec_datetime));
            output.Add(new SqlParameterData("@input_listcount", SqlDbType.BigInt, this._input_listcount));
            output.Add(new SqlParameterData("@output_listcount", SqlDbType.BigInt, this._output_listcount));
            output.Add(new SqlParameterData("@input_rowcount", SqlDbType.BigInt, this._input_rowcount));
            output.Add(new SqlParameterData("@output_rowcount", SqlDbType.BigInt, this._output_rowcount));
            output.Add(new SqlParameterData("@error_messages", SqlDbType.VarChar, this._error_messages.ToString()));

            return output;
        }

        #endregion
    }
}
