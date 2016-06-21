using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Uploaders {

    /// <summary>
    /// An uploader which takes an Excel 2003 file including its full directory and uploads 
    /// it as a DataTable. 
    /// </summary>
    /// <remarks>
    /// The uploader uses an OLEDB engine to upload an Excel 2003 file to a datatable. It currently 
    /// uses the MS ACE engine. Null values are replaced with default values of the datatype.
    /// </remarks>
    internal class XlsListUploader : IListUploader {

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a <see cref="XlsListUploader"/>, which uploads a list based on a filename.
        /// including the directory.
        /// </summary>
        /// <param name="sourceName">The filename including directory of the file to be uploaded.</param>
        public XlsListUploader(string sourceName) {
            SourceName = sourceName;
            _listDirectory = Path.GetDirectoryName(sourceName);
            _connectionString = GetConnectionString();
            _selectString = GetSelectString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Uploads a list from an Excel 2003/2007 file using an OLE DB connection.
        /// TODO: Look into async
        /// TODO: Upload files with multiple periods
        /// </summary>
        /// <returns>A DataTable containing data from the list.</returns>
        public async Task<DataTable> UploadList() {

            return await Task.Run(() => {
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

                    // Remove nulls from the datatable and ensure they can't be added
                    RemoveNullsFromDataTable(dt);

                    // Remove the weird hex value from the start of the lists
                    if(dt.Columns[0].ColumnName.Contains("\xef\xbb\xbf")) {
                        dt.Columns[0].ColumnName = dt.Columns[0].ColumnName.Replace("\xef\xbb\xbf", "");
                    }

                    return dt;
                }
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the connection string used in the OLEDB connection, including the provider, data 
        /// source and any extended properties (such as the Excel version) used by the file.
        /// </summary>
        /// <returns>A string to be used by an OLEDB connection for uploading a file.</returns>
        private string GetConnectionString() {
            var connectionString = new StringBuilder();
            connectionString.Append("Provider=" + ConnectionProvider + "; ");
            connectionString.Append("Data Source = " + SourceName + "; ");
            connectionString.Append("Extended Properties = \"Excel 8.0;HDR=YES;FMT=Delimited\"");
            return connectionString.ToString();
        }

        /// <summary>
        /// Gets the select string used in the OLEDB connection.
        /// </summary>
        /// <returns>A string to be used by an OLEDB connection for uploading a file.</returns>
        private string GetSelectString() {
            var sheetName = GetFirstSheetInFile(_connectionString);
            return "SELECT * FROM [" + sheetName + "]";
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


        /// <summary>
        /// Get the first sheet in the excel file passed to the connection string
        /// </summary>
        /// <param name="connectionString">The connection string containing the excel file</param>
        /// <returns>A string containing one of the sheets in the file</returns>
        private static string GetFirstSheetInFile(string connectionString) {

            var tables = new List<string>();

            // Use the connection sent with the excel file contained in it
            using(var objConn = new OleDbConnection(connectionString)) {

                objConn.Open();

                // Get the list of sheets in the file
                var dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if(dt != null) {

                    // Loop through all the sheet names and add it to the string
                    tables.AddRange(from DataRow row in dt.Rows select row["TABLE_NAME"].ToString());
                }
            }

            // Return the first table that was added
            return tables[0];
        }

        #endregion
    }
}
