using System.Collections.Generic;
using System.Data;
using BIApps.ListToolbox.Model.Uploaders;

namespace BIApps.ListToolbox.Model.Operators {

    /// <summary> 
    /// A class which can be used to split an <see cref="UploadedList"/> into multiple objects 
    /// with no more than <see cref="_rowCountSplit"/> rows contained in them.
    /// </summary>
    public class RowCountSplitter {

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
        /// The number of rows to split the <see cref="UploadedList"/> into.
        /// </summary>
        private readonly int _rowCountSplit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a <see cref="RowCountSplitter"/> which can be used to split an 
        /// <see cref="UploadedList"/> into multiple objects with no more than <see cref="rowCount"/> 
        /// rows.
        /// </summary>
        /// <param name="listGroup">The <see cref="UploadedList"/> to split</param>
        /// <param name="rowCount">The maximum number of rows to split by</param>
        public RowCountSplitter(UploadedListGroup listGroup, int rowCount) {
            _listGroup = listGroup;
            _rowCountSplit = rowCount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Splits an <see cref="UploadedListGroup"/> into an <see cref="UploadedListGroup"/> with multiple 
        /// <see cref="UploadedList"/> objects split by a specified number of rows.
        /// </summary>
        public UploadedListGroup Operate() {
            var output = new UploadedListGroup();
            foreach (var list in _listGroup) {
                foreach (var split in SplitTable(list)) {
                    output.Add(split);
                }
            }
            return output;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Splits an <see cref="UploadedList"/> into an <see cref="UploadedListGroup"/> with multiple 
        /// <see cref="UploadedList"/> objects with a specified number of rows.
        /// </summary>
        /// <returns>
        /// An <see cref="UploadedListGroup"/> containing a group of <see cref="UploadedList"/> objects 
        /// with no more than <see cref="_rowCountSplit"/> rows.
        /// </returns>
        private IEnumerable<UploadedList> SplitTable(UploadedList list) {
            var output = new UploadedListGroup();
            int rowsInTable = list.ListDetails.Rows.Count;
            int outputTableCount = (rowsInTable-1) / _rowCountSplit + 1;
            int rowCounter = 0;

            // Loop through the rows in the loaded table and add the values
            // to the newly created tables.
            for (int i = 0; i < outputTableCount; i++) {
                int tableNumber = i;
                output.Add(new UploadedList(
                    new DataTableUploader(() => {
                        var dt = list.ListDetails.Clone();
                        dt.TableName = list.ListName + "_" + tableNumber;
                        for(int j = 0; j < _rowCountSplit && rowCounter < rowsInTable; j++) {
                            var dr = dt.NewRow();
                            for(int k = 0; k < list.ListDetails.Columns.Count; k++) {
                                dr[k] = list.ListDetails.Rows[rowCounter][k];
                            }
                            dt.Rows.Add(dr);
                            rowCounter++;
                        }
                        return dt;
                    })));
            }

            return output;
        }

        #endregion
    }
}
