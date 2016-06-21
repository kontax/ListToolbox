using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Data.SqlClient;

namespace BIApps.ListToolbox {

    public class CustomerList : DataTable {

        private int priority;

        public int Priority {
            get { return priority; }
            set { priority = value; }
        }

        public CustomerList() : base() { }

        public CustomerList(string s) : base(s) { }


        /// <summary>
        /// Gets a CSV from a location and outputs it
        /// into a DataTable for processing. It uses the OLEDB connection
        /// to pull the file info via a SELECT query.
        /// </summary>
        /// <param name="FileName">CSV File to be input</param>
        /// <returns>DataTable with the info from the CSV</returns>
        public static CustomerList GetCSV(string FileName) {

            string fileNameNoDirectory = Path.GetFileName(FileName);
            string fileNameNoDirectoryOrExtension = Path.GetFileNameWithoutExtension(FileName);
            string fileExtension = Path.GetExtension(FileName);
            string directory = Path.GetDirectoryName(FileName);
            string schemaFileName = directory + "\\schema.ini";

            // In case we need to rename the file, save the filename
            string oldFileName = FileName;
            bool fileChanged = false;

            // Check to see if valid filenames are being uploaded
            List<string> supportedExtensions = new List<string> { ".csv", ".txt", ".tab", ".xls", ".xlsx" };
            var validFile = from f in supportedExtensions.AsEnumerable()
                            where f.Contains(fileExtension.ToLower())
                            select f.ToString();

            if(validFile.Count() == 0) {
                StringBuilder ext = new StringBuilder();
                foreach(string s in supportedExtensions)
                    ext.Append(s + ", ");
                throw new Exception(
                    "Invalid file type. The list of supported files are as follows: \n" +
                    ext.ToString() + "please convert the files to this type.");
            }


            // Rename files with multiple periods - this is
            // unsupported by the ACE Engine
            int periodCount = fileNameNoDirectory.Count(c => c == '.');
            if(periodCount > 1) {
                FileName = directory + "\\" + fileNameNoDirectoryOrExtension.Replace('.', '_') + fileExtension;
                try {
                    File.Move(oldFileName, FileName);
                } catch(IOException ex) {
                    string fileOpen = "Cannot create a file when that file already exists.\r\n";
                    if (ex.Message != fileOpen) {
                        throw ex;
                    } else {
                    throw new IOException(
                        "The application does not support files with periods in the filename.\n" +
                        "The file is usually renamed, however there is already a file with\n" +
                        "the new name specified. Please rename the file to remove periods\n" +
                        "and try again.");
                    }
                }
                fileChanged = true;
            }


            // Guess the delimiter used in the file and add a schema.ini
            if(fileExtension == ".csv" || fileExtension == ".txt") {
                char delimiter = MostCommonDelimiter(FileName);
                CreateSchemaFile(delimiter, schemaFileName, fileNameNoDirectory);
            }

            // Open an OLEDB connection to read from the file
            Dictionary<string, string> connection = new Dictionary<string, string>();
            connection = GetConnectionSelectStrings(FileName);

            OleDbConnection conn = new OleDbConnection(connection["Connection"]);

            conn.Open();

            // Select all the fields from the file
            OleDbDataAdapter adapter = new OleDbDataAdapter(connection["Select"].ToString(), conn);

            // Add it to a CustomerList using the filename without the extention as the TableName
            CustomerList dt = new CustomerList(Path.GetFileNameWithoutExtension(FileName));

            // Get the schema and convert everything to a string
            //adapter.FillSchema(dt, SchemaType.Mapped);
            //foreach(DataColumn dc in dt.Columns) {
            //    dc.DataType = typeof(string);
            //}

            try {
                adapter.Fill(dt);
            } catch(Exception) {
                throw;
            } finally {
                // Clean up
                if(conn != null) {
                    conn.Close();
                    conn.Dispose();
                }
                // Remove the schema.ini file if it exists
                File.Delete(schemaFileName);

                // Rename the file back if it was changed
                if(fileChanged) File.Move(FileName, oldFileName);
            }

            // Remove nulls from the datatable and ensure they can't be added
            RemoveNullsFromCustomerList(dt);
            foreach(DataColumn dc in dt.Columns) {
                dc.AllowDBNull = false;
            }

            // Remove the weird hex value from the start of the lists
            if(dt.Columns[0].ColumnName.Contains("\xef\xbb\xbf")) {
                dt.Columns[0].ColumnName = dt.Columns[0].ColumnName.Replace("\xef\xbb\xbf", "");
            }

            return dt;
        }


        /// <summary>
        /// Pulls a CustomerList and outputs it into a CSV file. The full path
        /// is needed to be passed into the function, not just the filename.
        /// </summary>
        /// <param name="FileName">Full path output</param>
        /// <param name="keepHeaders">Flag to say whether to export headers or not</param>
        public void OutputCSV(string FileName, bool keepHeaders = true) {

            StringBuilder sb = new StringBuilder();

            // Extract the columms and escape dodgy characters
            IEnumerable<string> columnNames = this.Columns.Cast<DataColumn>().Select(
                column => string.Concat("\"", column.ColumnName.ToString().Replace("\"", "\"\""), "\""));

            // Add the headers to the output if selected
            if(keepHeaders) {
                sb.AppendLine(string.Join(",", columnNames));
            }

            foreach(DataRow dr in this.Rows) {
                IEnumerable<string> fields = dr.ItemArray.Select(
                    field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            File.WriteAllText(FileName, sb.ToString());
        }


        /// <summary>
        /// Copy a DataTable into a CustomerList.
        /// </summary>
        /// <param name="dt">DataTable to convert.</param>
        /// <param name="tableName">Optional name to give the table.</param>
        /// <returns>The columns/rows in a CustomerList class</returns>
        public static CustomerList DataTableToCustomerList(DataTable dt, string tableName = "DefaultName") {
            CustomerList cl = new CustomerList(tableName);
            foreach(DataColumn tempColumn in dt.Columns) {
                cl.Columns.Add(tempColumn.ColumnName, tempColumn.DataType);
            }
            foreach(DataRow tempRow in dt.Rows) {
                cl.ImportRow(tempRow);
            }
            return cl;
        }


        /// <summary>
        /// Outputs a CustomerList based on a list of selected column headers
        /// </summary>
        /// <param name="columnName">The list of columns selected</param>
        /// <returns>
        /// A CustomerList with only the selected columns output.
        /// </returns>
        public CustomerList ColumnSelect(List<string> columnNames) {
            CustomerList output = new CustomerList();
            output.TableName = this.TableName;

            // Loop through each of the columns in the current CustomerList
            // and check to see if we have a match
            foreach(DataColumn dc in this.Columns) {
                foreach(string col in columnNames) {
                    if(dc.ColumnName == col) {
                        // If there's a match, add the column to the new CL and
                        // add each row from that column to the new list
                        output.Columns.Add(col, this.Columns[col].DataType);
                        for(int i = 0; i < this.Rows.Count; i++) {
                            if(output.Rows.Count == i) {
                                DataRow dr = output.NewRow();
                                output.Rows.Add(dr);
                            }
                            output.Rows[i][col] = this.Rows[i][col];
                        }
                    }
                }
            }

            return output;
        }


        /// <summary>
        /// Splits a CustomerList into multiple CustomerLists based on
        /// a rowcount, returning multiple CustomerLists in a CustomerListSet.
        /// </summary>
        /// <param name="rowCount">The maximum number of rows an output list can have</param>
        /// <returns>
        /// A CustomerList with multiple tables named after the value of 
        /// the column being split by
        /// </returns>
        public CustomerListSet SplitTable(int rowCount) {
            CustomerListSet output = new CustomerListSet();
            int rowsInTable = this.Rows.Count;
            int outputTableCount = rowsInTable / rowCount + 1;

            // Create the required number of tables and add them to the output.
            for(int i = 0; i < outputTableCount; i++) {
                CustomerList cl = CustomerList.DataTableToCustomerList(this.Clone());
                cl.TableName = this.TableName + "_" + i;
                output.Add(cl);
            }

            // Loop through the rows in the loaded table and add the values
            // to the newly created tables.
            int rowCounter = 0;
            foreach(DataRow inputRow in this.Rows) {
                int tableNumber = rowCounter / rowCount;
                DataRow dr = output.Tables[tableNumber].NewRow();
                for(int k = 0; k < this.Columns.Count; k++) {
                    dr[k] = this.Rows[rowCounter][k];
                }
                output.Tables[tableNumber].Rows.Add(dr);
                rowCounter++;
            }

            return output;
        }


        /// <summary>
        /// Splits a CustomerList into multiple CustomerLists based on 
        /// a column, returning multiple CustomerLists in a CustomerListsSet.
        /// Each CustomerList is named after the value in the splitting column.
        /// </summary>
        /// <param name="columnName">The string name of the column to split by</param>
        /// <returns>
        /// A CustomerList with multiple tables named after the value of 
        /// the column being split by
        /// </returns>
        public CustomerListSet SplitTable(string columnName) {
            CustomerListSet output = new CustomerListSet();

            // Get the column number being split so as to be able
            // to name the DataTables later on in the method.
            int colNumber = 0;
            foreach(DataColumn col in this.Columns) {
                if(col.ColumnName.Equals(columnName)) {
                    colNumber = col.Ordinal;
                }
            }

            // Using LINQ to split the tables, as well as getting the 
            // datatype of the column to be split to pass into the Field.
            Type dataType = this.Columns[colNumber].DataType;
            List<DataTable> dtResult = this.AsEnumerable()
                .GroupBy(row => row.Field<dynamic>(columnName))
                .Select(g => g.CopyToDataTable())
                .ToList();

            // Convert the results to from a DataTable to a CustomerList
            List<CustomerList> result = new List<CustomerList>();
            foreach(DataTable dt in dtResult) {
                result.Add(DataTableToCustomerList(dt, dt.TableName));
            }

            foreach(CustomerList d in result) {
                DataColumn col = d.Columns[colNumber];
                DataRow row = d.Rows[1];
                d.TableName = this.TableName + " - " + row[col.ColumnName].ToString();
                output.Tables.Add(d);
            }
            return output;
        }


        /// <summary>
        /// Take a list and return only those customers who are contactable based on parameter selections.
        /// </summary>
        /// <param name="procName">The stored procedure to call</param>
        /// <param name="spParams">The list of parameters to send to the proc</param>
        /// <param name="joinColumns">The columns available to join on in the DW</param>
        /// <returns>A customer list only showing either contactable customers or a flag splitting them</returns>
        public CustomerList Contactable(string procName, List<SqlParameterData> spParams, List<string> joinColumns) {

            CustomerList custList = new CustomerList();

            string listColumn = "", joinColumn = "", returnFlag = "";

            // Check the parameters and extract the columns in the list to 
            // join on in the DW, and whether to remove or flag the customers.
            foreach(SqlParameterData sp in spParams) {
                if(sp.Name == "@LIST_COLUMN")
                    listColumn = sp.Value.ToString();
                if(sp.Name == "@JOIN_COLUMN")
                    joinColumn = sp.Value.ToString();
                if(sp.Name == "@RETURN_FLAG")
                    returnFlag = sp.Value.ToString();
            }


            /* The following section adds customers to a temp table to pass 
             * to the proc. The table is a set type setup in the DW - currently
             * it's called scratch.cit.cust_list(customer_nk, ppo_cust_id, 
             * username, email), but will need to be added to the bi_apps DW
             * for production.
             */

            // Add the columns to join on to a temp table to pass to the proc
            foreach(string joinCol in joinColumns) {
                custList.Columns.Add(joinCol);
            }

            // The parameter name of the temp table to pass to the proc
            string tableParamName = "@CUSTOMER_LIST";

            // Add the customers to the temp table to pass to the proc
            int counter = 0;
            foreach(DataRow dr in this.Rows) {
                custList.NewRow();
                custList.Rows.Add(dr[listColumn]);
                custList.Rows[counter][0] = DBNull.Value;
                custList.Rows[counter][joinColumn] = dr[listColumn];
                counter++;
            }

            // Remove dupes
            custList = CustomerList.DataTableToCustomerList(custList.AsEnumerable().Distinct().CopyToDataTable());

            // Add the new table to the list of parameters being sent to the proc
            if(spParams.Count == 11) {
                spParams.RemoveAt(10);
            }
            spParams.Add(new SqlParameterData(tableParamName, SqlDbType.Structured, custList));


            CustomerList contactable = DBCommon.ExecuteStoredProcedure(procName, spParams, ConnectionType.ConnectionStringReadOnly);
            custList.Dispose();

            CustomerList output = this.CompareLists(contactable, listColumn, returnFlag);
            contactable.Dispose();

            output.TableName = this.TableName;
            return output;
        }


        /// <summary>
        /// Compare two lists and output only those rows that join on a specific column.
        /// </summary>
        /// <param name="comparison">The list to compare against.</param>
        /// <param name="column">The column to use as a comparison</param>
        /// <param name="compType">
        /// Either Flag (keep all columns and add a flag column from the comparison list)
        /// or Remove (only return the matching rows).
        /// </param>
        /// <returns>A CustomerList with only matching rows.</returns>
        private CustomerList CompareLists(CustomerList comparison, string column, string compType) {
            CustomerList output = new CustomerList();

            if(compType == "Remove") {
                // This only returns the matching rows
                return CustomerList.DataTableToCustomerList(
                            (from i in this.AsEnumerable()
                             join c in comparison.AsEnumerable()
                                 on i.Field<dynamic>(column)
                                 equals c.Field<dynamic>(column)
                                 into j
                             from x in j.DefaultIfEmpty()
                             where x != null
                             select i).CopyToDataTable());
            } else {
                // Return the original list plus a flag column
                try {
                    this.PrimaryKey = new DataColumn[] { this.Columns[column] };
                } catch(ArgumentException) {
                    throw new ArgumentException("The list contains duplicate rows. Please remove these before "
                                                + "checking the lists for contactibility.");
                }
                comparison.PrimaryKey = new DataColumn[] { comparison.Columns[column] };
                output = CustomerList.DataTableToCustomerList(this.Copy(), this.TableName);
                output.Merge(comparison, false, MissingSchemaAction.Add);
                output.AcceptChanges();
            }

            return output;
        }


        /// <summary>
        /// Convert all the columns in a CustomerList to type string - this is used so as
        /// to avoid errors when merging lists with different types as columns.
        /// </summary>
        /// <returns>CustomerList with all columns as string</returns>
        public CustomerList ConvertColumnsToText() {
            CustomerList output = new CustomerList();
            foreach(DataColumn dc in this.Columns) {
                output.Columns.Add(dc.ColumnName, typeof(string));
            }
            this.CopyCustomerListWithoutSchema(output);
            return output;
        }


        /// <summary>
        /// Private method used to copy the data in a CustomerList to another CustomerList
        /// without copying over the schema.
        /// </summary>
        /// <param name="output">The CustomerList with the same schema as this CL</param>
        private void CopyCustomerListWithoutSchema(CustomerList output) {
            int rowCount = this.Rows.Count;
            int rowCounter = 0;
            foreach(DataRow inputRow in this.Rows) {
                DataRow dr = output.NewRow();
                for(int k = 0; k < this.Columns.Count; k++) {
                    dr[k] = this.Rows[rowCounter][k];
                }
                output.Rows.Add(dr);
                rowCounter++;
            }
        }


        /// <summary>
        /// Look through the file and see if a valid delimiter can be established from the first
        /// X lines of the file - it just looks to see what the most common character is and 
        /// returns that. If the most common character is not within the default list provided
        /// then the method will re-run with 10 more lines until a maximum of 100 lines is reached.
        /// The default list is as follows:
        ///     ,   (comma)
        ///     |   (pipe)
        ///     ;   (semicolon)
        ///     :   (colon)
        ///     \t  (tab)
        /// </summary>
        /// <param name="filename">Full path of the file to search through</param>
        /// <returns>A char denoting the delimiter found, defaulting to a comma if none found</returns>
        private static char MostCommonDelimiter(string filename, int linesToCheck = 10) {
            char delimiter;
            char[] listOfValidDelimiters = { ',', '|', ';', '\t', ':' };
            var reader = new StreamReader(filename);

            // Run through the first X lines of the file and append it to a StringBuilder object
            StringBuilder firstXLines = new StringBuilder();
            for(int i = 0; i < linesToCheck; i++) {
                firstXLines.Append(reader.ReadLine());
            }

            // Get the most commonly occuring character
            delimiter = firstXLines.ToString().GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;

            // Check to see if the character is contained in the list above
            if(!listOfValidDelimiters.Contains(delimiter)) {
                // If not run it again up to 100 lines
                linesToCheck += 10;
                if(linesToCheck < 100) {
                    reader.Close();
                    delimiter = MostCommonDelimiter(filename, linesToCheck);
                } else {
                    // Otherwise just default to a comma
                    delimiter = ',';
                };
            }

            reader.Close();

            return delimiter;
        }


        /// <summary>
        /// Create a schema.ini file so the OLEDB driver can differentiate between delimiters
        /// </summary>
        /// <param name="delimiter">The delimiter used in the file</param>
        /// <param name="schemaFileName">The name of the file to save the schema to</param>
        /// <param name="fileName">The file that the schema applies to</param>
        private static void CreateSchemaFile(char delimiter, string schemaFileName, string fileName) {
            StreamWriter writer = new StreamWriter(schemaFileName);
            FileInfo schemaFileInfo = new FileInfo(schemaFileName);

            // Add the filename to the top of the schema file
            writer.WriteLine("[" + fileName + "]\n");

            // Choose the delimiter in the schema.ini file here
            string delimText = "";
            switch(delimiter) {
                case ',':
                    delimText = "CSVDelimited";
                    break;
                case '\t':
                    delimText = "TabDelimited";
                    break;
                default:
                    delimText = "Delimited(" + delimiter + ")";
                    break;
            }

            // Add the format to the schema
            writer.WriteLine("Format=" + delimText);

            // Whether the file has column headers or not
            // TODO: Make ColHeaders an option using default headers (unless replacing with CSV parser)
            writer.WriteLine("ColNameHeader=True");
            schemaFileInfo.Attributes |= FileAttributes.Hidden;
            writer.Close();
        }


        /// <summary>
        /// Get the first sheet in the excel file passed to the connection string
        /// </summary>
        /// <param name="connectionString">The connection string containing the excel file</param>
        /// <returns>A string containing one of the sheets in the file</returns>
        private static string GetFirstSheetInFile(string connectionString) {
            OleDbConnection objConn = null;
            DataTable dt = null;

            try {
                // Use the connection sent with the excel file contained in it
                objConn = new OleDbConnection(connectionString);
                objConn.Open();

                // Get the list of sheets in the file
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string[] tables = new string[dt.Rows.Count];

                // Loop through all the sheet names and add it to the string
                int i = 0;
                foreach(DataRow row in dt.Rows) {
                    tables[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                // Return the first table that was added
                return tables[0];
            } catch(Exception) {
                throw;
            } finally {
                // Clean up
                if(objConn != null) {
                    objConn.Close();
                    objConn.Dispose();
                }
                if(dt != null) {
                    dt.Dispose();
                }
            }
        }


        /// <summary>
        /// Get the connection and select strings for a file depending on what type of file it is
        /// </summary>
        /// <param name="FileName">The name of the file being passed.</param>
        /// <returns>
        /// A dictionary containing two keys:
        ///     "Connection" which contains the connection info for the OLEDB connection
        ///     "Select" which contains the select statement to be used on the file
        /// </returns>
        private static Dictionary<string, string> GetConnectionSelectStrings(string FileName) {
            Dictionary<string, string> output = new Dictionary<string, string>();

            string directory = Path.GetDirectoryName(FileName);
            string filename = Path.GetFileName(FileName);
            string extension = Path.GetExtension(FileName);

            // The connection string used by the OLEDB connection
            StringBuilder connectionString = new StringBuilder();
            // The select string used on the file to populate a datatable
            StringBuilder selectString = new StringBuilder();

            // If an excel file is selected, this is the name of the sheet to select from
            string sheetName = null;

            // Use the ACE DB engine
            connectionString.Append("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = ");

            switch(extension.ToLower()) {
                case ".csv":
                case ".txt":
                case ".tab":
                    connectionString.Append(directory);
                    connectionString.Append("; Extended Properties = \"Text;HDR=YES;FMT=Delimited\"");
                    selectString.Append("SELECT * FROM [" + filename + "]");
                    break;
                case ".xls":
                    connectionString.Append(FileName);
                    connectionString.Append("; Extended Properties = \"Excel 8.0;HDR=YES;FMT=Delimited\"");
                    sheetName = GetFirstSheetInFile(connectionString.ToString());
                    selectString.Append("SELECT * FROM [" + sheetName + "]");
                    break;
                case ".xlsx":
                    connectionString.Append(FileName);
                    connectionString.Append("; Extended Properties = \"Excel 12.0;HDR=YES;FMT=Delimited\"");
                    sheetName = GetFirstSheetInFile(connectionString.ToString());
                    selectString.Append("SELECT * FROM [" + sheetName + "]");
                    break;
            }

            output.Add("Connection", connectionString.ToString());
            output.Add("Select", selectString.ToString());

            return output;
        }


        /// <summary>
        /// In the case of null values in a data table, this method
        /// will turn all nulls into zeros instead.
        /// </summary>
        public static CustomerList RemoveNullsFromCustomerList(CustomerList cl) {
            for(int a = 0; a < cl.Rows.Count; a++) {
                for(int i = 0; i < cl.Columns.Count; i++) {
                    if(cl.Rows[a][i] == DBNull.Value) {
                        Type type = cl.Columns[i].DataType;
                        if(type == typeof(int) || type == typeof(float) || type == typeof(double)) {
                            cl.Columns[i].ReadOnly = false;
                            cl.Rows[a][i] = 0.0F;
                        } else {
                            cl.Columns[i].ReadOnly = false;
                            cl.Rows[a][i] = "";
                        }
                    }
                }
            }

            return cl;
        }
    }
}
