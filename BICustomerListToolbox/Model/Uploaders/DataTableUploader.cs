using System;
using System.Data;

namespace BIApps.ListToolbox.Model.Uploaders {

    /// <summary>
    /// An uploader which uses a user defined function to return a DataTable.
    /// </summary>
    /// <remarks>
    /// This is mainly used to populate an <see cref="UploadedList"/> with a DataTable that 
    /// has been created from a method defined elsewhere in the application. It is mainly used 
    /// as a way to use the IListUploader to return a DataTable.
    /// </remarks>
    public class DataTableUploader : IListUploader {

        #region Properties & Fields

        /// <summary>
        /// The name of the file that is to be uploaded.
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// The DataTable to be returned when the UploadList() function is called.
        /// </summary>
        public DataTable DataTable { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="DataTableUploader"/> using a user specified Func to return
        /// a new DataTable.
        /// </summary>
        /// <param name="dataTableFunc">The function used to pull back a DataTable</param>
        public DataTableUploader(Func<DataTable> dataTableFunc ) {
            DataTable = dataTableFunc();
            SourceName = DataTable.TableName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the DataTable pulled using the user provided function.
        /// </summary>
        /// <returns>A new DataTable for the user to upload</returns>
        public DataTable UploadList() {
            return DataTable;
        }

        #endregion
    }
}
