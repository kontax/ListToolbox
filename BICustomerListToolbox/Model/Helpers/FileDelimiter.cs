using System.IO;
using System.Linq;
using System.Text;

namespace BIApps.ListToolbox.Model.Helpers {

    /// <summary>
    /// A simple class outlining the different delimiters used in a file, which contains a char delimiter
    /// as well as the delimiter name to be used in a schema.ini file.
    /// </summary>
    internal class FileDelimiter {

        #region Properties & Fields

        /// <summary>
        /// The list of known valid delimiters.
        /// </summary>
        private static readonly char[] ListOfValidDelimiters = { ',', '|', ';', '\t', ':' };

        /// <summary>
        /// The number of lines to check a file against for a valid delimiter in the GuessDelimiter() method.
        /// </summary>
        private const int LinesToCheck = 10;

        /// <summary>
        /// The max number of lines to check in the GuessDelimiter() method.
        /// </summary>
        private const int MaxLinesToCheck = 100;

        /// <summary>
        /// The default delimiter to return in the GuessDelimiter() method if no suitable one is found.
        /// </summary>
        private const char DefaultDelimiter = ',';

        /// <summary>
        /// The char delimiter used in text files to delimit columns.
        /// </summary>
        public char Delimiter { get; set; }

        /// <summary>
        /// The string name used in schema.ini files to outline the delimiter used in a file.
        /// </summary>
        public string DelimiterName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="FileDelimiter"/>.
        /// </summary>
        /// <param name="delimiter">The delimiter used to separate columns in a file.</param>
        public FileDelimiter(char delimiter) {
            Delimiter = delimiter;
            DelimiterName = GetDelimiterName();
        }

        /// <summary>
        /// A static method used to initialize a <see cref="FileDelimiter"/> based on the most common 
        /// character occuring within the first x number of lines in the file. If the most common 
        /// character isn't in the list of known delimiters, the default delimiter is returned.
        /// </summary>
        /// <param name="fileName">The full filename of the file to check</param>
        /// <returns>
        /// A new <see cref="FileDelimiter"/> of the most commonly occuring character, or the default
        /// delimiter (as per a private readonly field) if no delimiter is found.
        /// </returns>
        public static FileDelimiter GuessDelimiter(string fileName) {
            return new FileDelimiter(GuessDelimiter(fileName, LinesToCheck));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get a delimiter name for use in a <see cref="SchemaFile"/>.
        /// </summary>
        /// <returns></returns>
        private string GetDelimiterName() {
            switch(Delimiter) {
                case ',':
                    return "CSVDelimited";
                case '\t':
                    return "TabDelimited";
                default:
                    return "Delimited(" + Delimiter + ")";
            }
        }

        /// <summary>
        /// Method which loops through the lines in a file and tries to guess the delimiter in use
        /// by looking at the most commonly occuring character, assuming it is contained in the list
        /// of known delimiters (set as a private readonly field).
        /// </summary>
        /// <param name="fileName">The full filename of the text file to check</param>
        /// <param name="linesToCheck">The number of lines to test the file with</param>
        /// <returns>
        /// A char with the most common character in the file, or the default delimiter if
        /// the return value is not found.
        /// </returns>
        private static char GuessDelimiter(string fileName, int linesToCheck) {
            using(var reader = new StreamReader(fileName)) {

                // Run through the first X lines of the file and append it to a StringBuilder object
                var firstXLines = new StringBuilder();
                for(int i = 0; i < linesToCheck; i++) {
                    firstXLines.Append(reader.ReadLine());
                }

                // Get the most commonly occuring character
                char delimiter = firstXLines.ToString().GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;

                // Check to see if the character is contained in the list above
                if(!ListOfValidDelimiters.Contains(delimiter)) {
                    // If not run it again using another x number of lines
                    linesToCheck += LinesToCheck;
                    // If we have gone over the max, just return the default
                    if(linesToCheck >= MaxLinesToCheck) return DefaultDelimiter;
                    reader.Close();
                    return GuessDelimiter(fileName, linesToCheck);
                }
                return delimiter;
            }
        }

        #endregion
    }
}
