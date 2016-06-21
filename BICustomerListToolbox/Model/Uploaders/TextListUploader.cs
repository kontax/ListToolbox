using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using BIApps.ListToolbox.Model.Helpers;

namespace BIApps.ListToolbox.Model.Uploaders {

    /// <summary>
    /// An uploader which takes a text file including its full directory and uploads it as a DataTable. 
    /// </summary>
    /// <remarks>
    /// The uploader uses an OLEDB engine to upload a text file to a datatable. It uses a generated 
    /// schema.ini file to capture the information needed, and currently uses the MS ACE engine.
    /// Null values are replaced with default values of the datatype.
    /// </remarks>
    public class TextListUploader : IListUploader {

        #region Properties & Fields

        /// <summary>
        /// This is the connection string used for the OleDbConnector, which is currently
        /// defaulted at the MS ACE engine.
        /// TODO: The ConnectionProvider should be interchangeable (currently just the MS ACE OLEDB engine).
        /// </summary>
        private const string ConnectionProvider = "Microsoft.ACE.OLEDB.12.0";

        /// <summary>
        /// The name of the file that is to be uploaded.
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// The directory that the file to be uploaded is in.
        /// </summary>
        private readonly string _listDirectory;

        /// <summary>
        /// The connection string used to upload the file, depenedent on the provider and file.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// The select string used to upload the file.
        /// </summary>
        private readonly string _selectString;

        /// <summary>
        /// The <see cref="SchemaFile"/> used to outline the details of the file being uploaded.
        /// </summary>
        private readonly SchemaFile _schemaFile;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a <see cref="TextListUploader"/>, which uploads a list based on a filename.
        /// including the directory.
        /// </summary>
        /// <param name="sourceName">The filename including directory of the file to be uploaded.</param>
        public TextListUploader(string sourceName) {
            SourceName = sourceName;
            _listDirectory = Path.GetDirectoryName(sourceName);
            _schemaFile = new SchemaFile(sourceName, FileDelimiter.GuessDelimiter(sourceName));
            _connectionString = GetConnectionString();
            _selectString = GetSelectString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Uploads a list from a text file using an OLE DB connection.
        /// TODO: Look into async
        /// TODO: Upload files with multiple periods
        /// </summary>
        /// <returns>A DataTable containing data from the list.</returns>
        public DataTable UploadList() {

            using(var conn = new OleDbConnection(_connectionString))
            using(var adapter = new OleDbDataAdapter(_selectString, conn)) {

                // Add it to a DataTable using the filename without the extention as the TableName
                var dt = new DataTable(Path.GetFileNameWithoutExtension(SourceName));

                // Get the schema and convert everything to a string
                adapter.FillSchema(dt, SchemaType.Mapped);
                foreach(DataColumn dc in dt.Columns) {
                    dc.DataType = typeof(string);
                }

                // Populate the DataTable with the uploaded file
                adapter.Fill(dt);

                //Delete the schema file used
                _schemaFile.Delete();

                // Fix any potential issues with the DataTable
                FixDataTableIssues(dt);

                return dt;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the connection string used in the OLEDB connection, including the provider, data 
        /// source and any extended properties (such as the <see cref="FileDelimiter"/>) used by the file.
        /// </summary>
        /// <returns>A string to be used by an OLEDB connection for uploading a file.</returns>
        private string GetConnectionString() {
            var connectionString = new StringBuilder();
            connectionString.Append("Provider=" + ConnectionProvider + "; ");
            connectionString.Append("Data Source = " + _listDirectory + "; ");
            connectionString.Append("Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");
            return connectionString.ToString();
        }

        /// <summary>
        /// Gets the select string used in the OLEDB connection.
        /// </summary>
        /// <returns>A string to be used by an OLEDB connection for uploading a file.</returns>
        private string GetSelectString() {
            return "SELECT * FROM [" + Path.GetFileName(SourceName) + "]";
        }

        /// <summary>
        /// This method runs through a list of checks to remove any potential issues which may 
        /// occur further down the line when using the DataTable, eg. removing null values, or 
        /// the strange hex value at the start of some files pulled from SSMS.
        /// </summary>
        /// <param name="dt">The DataTable to check for issues</param>
        private static void FixDataTableIssues(DataTable dt) {
            RemoveNullsFromDataTable(dt);
            RemoveHexValues(dt);
        }

        /// <summary>
        /// Occasionally a DataTable will have the hex values "\xef\xbb\xbf" as the very first 
        /// characters, which causes issues when trying to match column names. This method removes 
        /// those values from the DataTable.
        /// </summary>
        /// <param name="dt">The DataTable to check for issues</param>
        private static void RemoveHexValues(DataTable dt) {
            // Remove the weird hex value from the start of the lists
            if(dt.Columns[0].ColumnName.Contains("\xef\xbb\xbf")) {
                dt.Columns[0].ColumnName = dt.Columns[0].ColumnName.Replace("\xef\xbb\xbf", "");
            }
        }

        /// <summary>
        /// Runs through all cells of a DataTable and replaces any null values to prevent issues
        /// further down the line.
        /// TODO: Find a faster way to implement RemoveNullsFromDataTable()
        /// </summary>
        /// <param name="dt">The DataTable which is to have null values removed from its cells.</param>
        private static void RemoveNullsFromDataTable(DataTable dt) {
            for(int a = 0; a < dt.Rows.Count; a++) {
                for(int i = 0; i < dt.Columns.Count; i++) {
                    if(dt.Rows[a][i] == DBNull.Value) {
                        Type type = dt.Columns[i].DataType;
                        if(type == typeof(int) || type == typeof(float) || type == typeof(double)) {
                            dt.Columns[i].ReadOnly = false;
                            dt.Rows[a][i] = 0.0F;
                        } else {
                            dt.Columns[i].ReadOnly = false;
                            dt.Rows[a][i] = "";
                        }
                    }
                }
            }

            foreach(DataColumn dc in dt.Columns) {
                dc.AllowDBNull = false;
            }
        }

        #endregion
    }
}
