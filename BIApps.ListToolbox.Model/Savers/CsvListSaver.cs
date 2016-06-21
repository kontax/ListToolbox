using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BIApps.ListToolbox.ListHelpers.Savers {

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
        
        #region Public Methods

        /// <summary>
        /// Save a CSV file to disk using the supplied name and directory. Characters which could 
        /// cause issues are escaped before saving.
        /// </summary>
        /// <param name="list">The <see cref="UploadedList"/> to save to disk</param>
        /// <param name="listPath">The directory to save the list to</param>
        /// <param name="keepHeaders">Whether to keep the headers in the output file or not</param>
        public void SaveList(UploadedList list, string listPath, bool keepHeaders) {

            var sb = new StringBuilder();
            var listName = "[out] " + list.ListName;
            var fullListPath = listPath + "\\" + listName + ".csv";

            // Extract the columms and escape dodgy characters
            var columnNames = list.ListDetails.Columns.Cast<DataColumn>().Select(
                column => string.Concat("\"", column.ColumnName.Replace("\"", "\"\""), "\""));

            // Add the headers to the output if selected
            if(keepHeaders) {
                sb.AppendLine(string.Join(",", columnNames));
            }

            // Loop through all the fields and add them to the StringBuilder
            foreach(var fields in from DataRow dr in list.ListDetails.Rows
                                  select dr.ItemArray.Select(
                                      field => string.Concat("\"", field
                                                     .ToString()
                                                     .Replace("\"", "\"\""), "\""))) {
                sb.AppendLine(string.Join(",", fields));
            }

            // Output the list to a directory
            File.WriteAllText(fullListPath, sb.ToString());
        }

        /// <summary>
        /// Save a CSV file to disk using the supplied name and directory. Characters which could 
        /// cause issues are escaped before saving.
        /// </summary>
        /// <param name="lists">The <see cref="UploadedListGroup"/> to save to disk</param>
        /// <param name="listPath">The directory to save the list to</param>
        /// <param name="keepHeaders">Whether to keep the headers in the output file or not</param>
        public void SaveLists(UploadedListGroup lists, string listPath, bool keepHeaders) {
            foreach(var list in lists) {
                SaveList(list, listPath, keepHeaders);
            }
        }

        #endregion
    }
}
