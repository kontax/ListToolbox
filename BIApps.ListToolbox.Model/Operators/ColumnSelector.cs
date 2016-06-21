using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Operators {

    /// <summary>
    /// Initializes a <see cref="ColumnSelector"/> which takes in an <see cref="UploadedList"/> 
    /// and a user selected list of columns. Once initialized, the user can call the 
    /// OutputSelectedColumns() method in order to get an <see cref="UploadedList"/> containing 
    /// only those columns selected. 
    /// </summary>
    public class ColumnSelector : IListOperation {

        #region Properties & Fields

        private readonly UploadedListGroup _listGroup;

        /// <summary>
        /// An <see cref="UploadedListGroup"/> containing the <see cref="UploadedList"/> objects to 
        /// be deduped.
        /// </summary>
        public UploadedListGroup UploadedListGroup {
            get { return _listGroup; }
        }

        /// <summary>
        /// The columns selected to output a new <see cref="UploadedList"/> object.
        /// TODO: Should this ColumnsSelector selected columns be a List of DataColumns?
        /// </summary>
        private readonly List<string> _selectedColumns;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a <see cref="ColumnSelector"/> which takes in an <see cref="UploadedList"/> 
        /// and a user selected list of columns. Once initialized, the user can call the 
        /// OutputSelectedColumns() method in order to get an <see cref="UploadedList"/> containing 
        /// only those columns selected.
        /// </summary>
        /// <param name="list">The <see cref="UploadedList"/> which selected columns are wanted from</param>
        /// <param name="selectedColumns">The list of selected columns to output</param>
        public ColumnSelector(UploadedListGroup list, List<string> selectedColumns) {
            _listGroup = list;
            _selectedColumns = selectedColumns;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Outputs a DataTable based on a list of selected column headers
        /// </summary>
        /// <returns>
        /// A DataTable with only the selected columns output.
        /// </returns>
        public async Task<UploadedListGroup> Operate() {

            var listGroup = new UploadedListGroup {
                FilePath = _listGroup.FilePath
            };

            await Task.Run(() => {

                foreach(var list in _listGroup) {
                    var output = new DataTable { TableName = list.ListName };

                    // Loop through each of the columns in the current CustomerList
                    // and check to see if we have a match
                    foreach(var col in from DataColumn dc in list.ListDetails.Columns
                                       from col in _selectedColumns
                                       where dc.ColumnName == col
                                       select col) {

                        // If there's a match, add the column to the new CL and
                        // add each row from that column to the new list
                        output.Columns.Add(col, list.ListDetails.Columns[col].DataType);
                        for(var i = 0; i < list.ListDetails.Rows.Count; i++) {
                            if(output.Rows.Count == i) {
                                var dr = output.NewRow();
                                output.Rows.Add(dr);
                            }
                            output.Rows[i][col] = list.ListDetails.Rows[i][col];
                        }
                    }
                    listGroup.Add(new UploadedList(output));
                }
            });

            return listGroup;
        }

        #endregion
    }
}
