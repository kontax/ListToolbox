using System;
using System.IO;
using BIApps.ListToolbox.ListHelpers.Uploaders;

namespace BIApps.ListToolbox.ListHelpers.Helpers {

    /// <summary>
    /// A schema.ini file containing the upload information needed for <see cref="IListUploader"/>
    /// </summary>
    /// <remarks>
    /// A hidden file is created in the directory specified which contains information needed to upload
    /// a list, such as the delimiter, the format and whether it contains column headers or not.
    /// </remarks>
    internal class SchemaFile : IDisposable {

        #region Properties & Fields

        /// <summary>
        /// The filename (without directory) that the schema file is called - defaulted to schema.ini
        /// </summary>
        private const string SchemaFileName = "schema.ini";

        /// <summary>
        /// The file that is to be uploaded, which <see cref="SchemaFile"/> contains the info of.
        /// </summary>
        private readonly string _sourceFileName;

        /// <summary>
        /// The filename including directory information of the schema file to create.
        /// </summary>
        private readonly string _schemaFileName;

        /// <summary>
        /// The <see cref="FileDelimiter"/> the file to upload is delimited by.
        /// </summary>
        private readonly FileDelimiter _fileDelemeter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of a <see cref="SchemaFile"/> in order to create a schema.ini file.
        /// </summary>
        /// <param name="sourceFileName">The filename including directory of the file to be uploaded.</param>
        /// <param name="fileDelimiter">The <see cref="FileDelimiter"/> the file to upload is delimited by.</param>
        public SchemaFile(string sourceFileName, FileDelimiter fileDelimiter) {
            _sourceFileName = sourceFileName;
            _schemaFileName = Path.GetDirectoryName(sourceFileName) + "\\" + SchemaFileName;
            _fileDelemeter = fileDelimiter;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a schema file in the directory of the list to be uploaded.
        /// </summary>
        public void Create() {

            // If the schema file is already there, delete it
            if(File.Exists(_schemaFileName)) Delete();

            using(var writer = new StreamWriter(_schemaFileName)) {

                var schemaFileInfo = new FileInfo(_schemaFileName);

                // Add the filename to the top of the schema file
                writer.WriteLine("[" + _sourceFileName + "]\n");

                // Add the format to the schema
                writer.WriteLine("Format=" + _fileDelemeter.DelimiterName);

                // Whether the file has column headers or not
                writer.WriteLine("ColNameHeader=True");

                // Make the file hidden so it doesn't show up in the folder to the user
                schemaFileInfo.Attributes |= FileAttributes.Hidden;
            }
        }

        public void Delete() {
            File.Delete(_schemaFileName);
        }

        public void Dispose() {
            Delete();
        }

        #endregion
    }
}
