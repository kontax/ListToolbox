using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIContextMenu {
    public static class FilePreview {

        /// <summary>
        /// Loop through all the files selected and return the list of columns in common
        /// </summary>
        /// <param name="selectedItems">The list of files to check against</param>
        /// <returns>A List containing common headers in the selected files</returns>
        public static List<string> GetCommonHeaders(IEnumerable<string> selectedItems) {
            
            List<string> colsInCommon = new List<string>();
            Dictionary<string, int> columnCount = new Dictionary<string, int>();
            int fileCount = 0;

            // Loop through the list of files provided and get the common headers
            foreach(string file in selectedItems) {
                fileCount++;
                foreach(string header in FilePreview.GetHeadersFromFile(file)) {
                    if(!columnCount.ContainsKey(header)) {
                        columnCount.Add(header, 1);
                    } else {
                        columnCount[header]++;
                    }
                }
            }

            // Add each header which is contained in all files to the output list
            foreach(KeyValuePair<string, int> x in columnCount) {
                if(x.Value == fileCount) {
                    colsInCommon.Add(x.Key.Replace("\"", ""));
                }
            }

            return colsInCommon;
        }

        /// <summary>
        /// Get the headers from a file
        /// </summary>
        /// <param name="filename">The filename to check</param>
        /// <returns>A list of columns in the first row of the file</returns>
        private static List<string> GetHeadersFromFile(string filename) {
            List<string> headers = new List<string>();

            string line;
            string[] row;

            using(StreamReader reader = new StreamReader(filename)) {
                line = reader.ReadLine();
            }

            char delimiter = MostCommonDelimiter(filename);

            row = line.Split(delimiter);

            foreach(string s in row) {
                headers.Add(s);
            }

            return headers;
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
        ///         (space)
        /// </summary>
        /// <param name="filename">Full path of the file to search through</param>
        /// <returns>A char denoting the delimiter found, defaulting to a comma if none found</returns>
        private static char MostCommonDelimiter(string filename, int linesToCheck = 10) {
            char delimiter;
            char[] listOfValidDelimiters = { ',', '|', ';', '\t', ':', ' ' };
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
    }
}
