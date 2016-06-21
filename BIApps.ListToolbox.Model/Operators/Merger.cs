using System;
using System.Data;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Operators {

    /// <summary>
    /// Instantiates a merger which takes an <see cref="UploadedListGroup"/> and merges all the 
    /// <see cref="UploadedList"/> objects into one <see cref="UploadedList"/>. 
    /// </summary>
    /// <remarks>
    /// The merger uses the DataTables Merge() method, and if column names are of a different case,
    /// the columns of new DataTables are pasted on the end.
    /// </remarks>
    public class Merger : IListOperation {

        #region Properties & Fields

        private readonly UploadedListGroup _listGroup;

        /// <summary>
        /// An <see cref="UploadedListGroup"/> containing the <see cref="UploadedList"/> objects to 
        /// be deduped.
        /// </summary>
        public UploadedListGroup UploadedListGroup {
            get { return _listGroup; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates a merger which takes an <see cref="UploadedListGroup"/> and merges all the 
        /// <see cref="UploadedList"/> objects.
        /// </summary>
        /// <param name="listGroup">The <see cref="UploadedListGroup"/> which contains the lists to be merged</param>
        public Merger(UploadedListGroup listGroup) {
            _listGroup = listGroup;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Merges the <see cref="UploadedListGroup"/> within this <see cref="Merger"/>.
        /// </summary>
        /// <returns>A new merged <see cref="UploadedList"/></returns>
        public async Task<UploadedListGroup> Operate() {

            var mergedList = new DataTable("Merged List");

            await Task.Run(() => {
                CheckListGroupHeadingCase();
                foreach(var list in _listGroup) {
                    mergedList.Merge(list.ListDetails);
                }
            });


            var output = new UploadedListGroup {
                FilePath = _listGroup.FilePath
            };

            output.Add(new UploadedList(mergedList));

            return output;
        }

        #endregion

        #region Private Methods

        private void CheckListGroupHeadingCase() {
            for (int i = 0; i < _listGroup.UploadedLists.Count; i++) {
                for (int j = i + 1; j < _listGroup.UploadedLists.Count; j++) {
                    foreach (DataColumn col1 in _listGroup[i].ListDetails.Columns) {
                        foreach (DataColumn col2 in _listGroup[j].ListDetails.Columns) {
                            if (String.Equals(col1.ColumnName, col2.ColumnName,
                                StringComparison.CurrentCultureIgnoreCase)) {
                                col2.ColumnName = col1.ColumnName;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
