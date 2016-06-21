using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Operators {

    /// <summary>
    /// Splits an <see cref="UploadedList"/> into an <see cref="UploadedListGroup"/> with multiple 
    /// <see cref="UploadedList"/> objects split by the distinct values of a selected column.
    /// </summary>
    /// <remarks>
    /// The output of the SplitTable() method takes the input list of the property and splits it into 
    /// multiple lists based on the distinct values in the selected column. For example, if one column
    /// had the values a, b, c for a group of rows, then 3 tables will be outputted, table a, b and c.
    /// </remarks>
    public class ColumnValueSplitter : IListOperation {

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
        /// The column name to split the <see cref="UploadedList"/> into, based on the distinct 
        /// values contained in the column.
        /// </summary>
        private readonly string _columnNameSplit;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a <see cref="ColumnValueSplitter"/> which can be used to split an 
        /// <see cref="UploadedList"/> into multiple objects based on the distinct values in
        /// the <see cref="columnName"/>.
        /// </summary>
        /// <param name="list">The <see cref="UploadedList"/> to split</param>
        /// <param name="columnName">The column to split the lists by</param>
        public ColumnValueSplitter(UploadedListGroup list, string columnName) {
            _listGroup = list;
            _columnNameSplit = columnName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Splits an <see cref="UploadedListGroup"/> into an <see cref="UploadedListGroup"/> with multiple 
        /// <see cref="UploadedList"/> objects split by the distinct values of a selected column.
        /// </summary>
        public async Task<UploadedListGroup> Operate() {

            var output = new UploadedListGroup {
                FilePath = _listGroup.FilePath
            };

            foreach(var list in _listGroup) {
                var splitTables = await SplitTable(list);
                foreach(var split in splitTables) {
                    output.Add(split);
                }
            }

            return output;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Splits an <see cref="UploadedList"/> into an <see cref="UploadedListGroup"/> with multiple 
        /// <see cref="UploadedList"/> objects split by the distinct values of a selected column.
        /// </summary>
        /// <returns>
        /// An <see cref="UploadedListGroup"/> containing a group of <see cref="UploadedList"/> objects 
        /// where each list contains only those rows where the selected column is a single value.
        /// </returns>
        private async Task<IEnumerable<UploadedList>> SplitTable(UploadedList list) {
            var output = new UploadedListGroup();

            // Get the distinct column values from the selected column in the DataTable
            var distinctColumnValues = await Task.Run(() => from col in list.ListDetails.AsEnumerable()
                                                            group col by col[_columnNameSplit]
                                                                into g
                                                                select g.Key);

            // Loop through each of the distinct values, and add a new UploadedList containing
            // a new DataTable with the distinct column values.

            await Task.Run(() => {
                foreach(string colValue in distinctColumnValues) {
                    var value = colValue;
                    var table = (from dt in list.ListDetails.AsEnumerable()
                                 where dt[_columnNameSplit].ToString() == value
                                 select dt).CopyToDataTable();
                    table.TableName = list.ListName + " - " + value;
                    output.Add(new UploadedList(table));
                }
            });

            return output;
        }

        #endregion
    }
}
