using System.ComponentModel;
using System.Data;

namespace BIApps.ListToolbox.ListHelpers {

    /// <summary>
    /// A list uploaded by the user to perform operations on from a file.
    /// </summary>
    /// <remarks>
    /// The user may upload a number of different file types such as csv, xlsx and txt. This
    /// is done using the ACE engine and a schema.ini file.
    /// </remarks>
    public class UploadedList : INotifyPropertyChanged {

        #region Properties & Fields

        private string _listName;
        private int _listRowCount;

        /// <summary>
        /// The DataTable which contains the information loaded for the list.
        /// </summary>
        public DataTable ListDetails { get; set; }

        /// <summary>
        /// The name of the list being uploaded
        /// </summary>
        public string ListName {
            get { return _listName; }
            set { _listName = value; RaisePropertyChanged("ListName"); }
        }

        /// <summary>
        /// The number of rows contained in the list.
        /// </summary>
        public int ListRowCount {
            get { return _listRowCount; }
            set { _listRowCount = value; RaisePropertyChanged("ListRowCount"); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="UploadedList"/> using a file.
        /// </summary>
        /// <param name="list">The datatable containing the list details</param>
        public UploadedList(DataTable list) {
            ListDetails = list;
            ListName = ListDetails.TableName;
            ListRowCount = ListDetails.Rows.Count;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the table name and row count for the uploaded list.
        /// </summary>
        /// <returns>A string with the name and row count of this uploaded list.</returns>
        public override string ToString() {
            return _listName + " :: " + _listRowCount;
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if(handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
