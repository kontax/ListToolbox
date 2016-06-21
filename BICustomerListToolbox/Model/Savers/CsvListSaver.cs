using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BIApps.ListToolbox.Model.Savers {

    /// <summary>
    /// Implements <see cref="IListSaver"/> to save an <see cref="UploadedList"/> to a comma delimited 
    /// file on disk.
    /// </summary>
    /// <remarks>
    /// The class takes in an <see cref="UploadedList"/> along with a directory to save, and outputs a csv 
    /// file with a prefix of "[out] " to the filename. Characters which could cause issues (such as \ 
    /// or ") are escaped to prevent formatting issues.
    /// </remarks>
    public class CsvListSaver : IListSaver {

        #region Properties & Fields

        /// <summary>
        /// The <see cref="UploadedList"/> to save to disk.
        /// </summary>
        private readonly UploadedList _list;

        /// <summary>
        /// Whether to keep headers in the file when saving or not.
        /// </summary>
        private readonly bool _keepHeaders;

        /// <summary>
        /// The full directory name, list name and extension used to save the file as.
        /// </summary>
        private readonly string _fullListPath;

        /// <summary>
        /// The name of the list being saved.
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// The directory to save the list to.
        /// </summary>
        public string ListPath { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an <see cref="IListSaver"/> used to save an <see cref="UploadedList"/> to disk 
        /// as a comma delimited file, prepending the list name with the string "[out] ".
        /// </summary>
        /// <param name="list">The <see cref="UploadedList"/> to save to disk</param>
        /// <param name="listPath">The directory to save the list to</param>
        /// <param name="keepHeaders">Whether to keep the headers in the output file or not</param>
        public CsvListSaver(UploadedList list, string listPath, bool keepHeaders) {
            _list = list;
            _keepHeaders = keepHeaders;
            ListName = "[out] " + list.ListName;
            ListPath = listPath;
            _fullListPath = ListPath + "\\" + ListName + ".csv";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Save a CSV file to disk using the supplied name and directory. Characters which could 
        /// cause issues are escaped before saving.
        /// </summary>
        public void SaveList() {

            var sb = new StringBuilder();

            // Extract the columms and escape dodgy characters
            var columnNames = _list.ListDetails.Columns.Cast<DataColumn>().Select(
                column => string.Concat("\"", column.ColumnName.Replace("\"", "\"\""), "\""));

            // Add the headers to the output if selected
            if(_keepHeaders) {
                sb.AppendLine(string.Join(",", columnNames));
            }

            // Loop through all the fields and add them to the StringBuilder
            foreach(var fields in from DataRow dr in _list.ListDetails.Rows
                                  select dr.ItemArray.Select(
                                      field => string.Concat("\"", field
                                                     .ToString()
                                                     .Replace("\"", "\"\""), "\""))) {
                sb.AppendLine(string.Join(",", fields));
            }

            // Output the list to a directory
            File.WriteAllText(_fullListPath, sb.ToString());
        }

        #endregion
    }
}
