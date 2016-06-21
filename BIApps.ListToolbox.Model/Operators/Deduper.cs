using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BIApps.ListToolbox.ListHelpers.Helpers;

namespace BIApps.ListToolbox.ListHelpers.Operators {

    /// <summary>
    /// Instantiates a deduper which takes an <see cref="UploadedListGroup"/> and dedupes all the 
    /// <see cref="UploadedList"/> objects by a selected dedupeColumn. 
    /// </summary>
    public class Deduper : IListOperation {

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
        /// The column contained in each of the <see cref="UploadedList"/> objects to dedupe by.
        /// </summary>
        private readonly string _dedupeColumn;

        /// <summary>
        /// Whether the deduping should take case sensitivity into account or not.
        /// </summary>
        private readonly Case _case;

        /// <summary>
        /// A temporary DataTable used to store the values already run through the algorithm.
        /// </summary>
        private DataTable _removeTable;

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiates a deduper which takes an <see cref="UploadedListGroup"/> and dedupes all the 
        /// <see cref="UploadedList"/> objects by a selected dedupeColumn.
        /// </summary>
        /// <param name="listGroup">The <see cref="UploadedListGroup"/> which contains the lists to be deduped</param>
        /// <param name="dedupeColumn">The column to dedupe by</param>
        /// <param name="caseSensitivity">Whether to take case sensitivity into account when deduping</param>
        public Deduper(UploadedListGroup listGroup, string dedupeColumn, Case caseSensitivity) {
            _listGroup = listGroup;
            _dedupeColumn = dedupeColumn;
            _case = caseSensitivity;
            GetRemoveList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dedupes the <see cref="UploadedListGroup"/> within this <see cref="Deduper"/>.
        /// </summary>
        /// <returns>A new <see cref="UploadedListGroup"/> with deduped <see cref="UploadedList"/> objects</returns>
        public async Task<UploadedListGroup> Operate() {

            var output = new UploadedListGroup {
                FilePath = _listGroup.FilePath
            };

            foreach(var list in _listGroup) {
                output.Add(_case == Case.Sensitive
                    ? await DeDupeCaseSensitive(list)
                    : await DeDupeCaseInsensitive(list));

                await SetRemovedRows(list);
            }

            _removeTable = null;
            return output;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dedupe a list taking case sensitivity into account.
        /// </summary>
        /// <param name="list">The list to dedupe</param>
        /// <returns>An <see cref="UploadedList"/> which has items from the removeList taken out of it</returns>
        private async Task<UploadedList> DeDupeCaseSensitive(UploadedList list) {

            var dt = new DataTable();

            await Task.Run(() => {
                var rows = (from c in list.ListDetails.AsEnumerable()
                            join b in _removeTable.AsEnumerable().Distinct()
                                on c.Field<dynamic>(_dedupeColumn)
                                equals b.Field<dynamic>(_dedupeColumn)
                                into j
                            from x in j.DefaultIfEmpty()
                            where x == null
                            select c).ToList();
                dt = rows.Any() ? rows.CopyToDataTable() : list.ListDetails.Clone();
                dt.TableName = list.ListName;
            });

            return new UploadedList(dt);
        }

        /// <summary>
        /// Dedupe a list without taking case sensitivity into account.
        /// </summary>
        /// <param name="list">The list to dedupe</param>
        /// <returns>An <see cref="UploadedList"/> which has items from the removeList taken out of it</returns>
        private async Task<UploadedList> DeDupeCaseInsensitive(UploadedList list) {

            var dt = new DataTable();

            await Task.Run(() => {
                var rows = (from c in list.ListDetails.AsEnumerable()
                            join b in _removeTable.AsEnumerable().Distinct()
                                on c.Field<dynamic>(_dedupeColumn).ToString().ToLower()
                                equals b.Field<dynamic>(_dedupeColumn).ToString().ToLower()
                                into j
                            from x in j.DefaultIfEmpty()
                            where x == null
                            select c).ToList();

                dt = rows.Any() ? rows.CopyToDataTable() : list.ListDetails.Clone();
                dt.TableName = list.ListName;
            });

            return new UploadedList(dt);
        }

        /// <summary>
        /// Create a _removeList to store the fields already run through.
        /// </summary>
        private void GetRemoveList() {
            _removeTable = new DataTable();
            var dedupeColumnType = _listGroup.UploadedLists[0].ListDetails.Columns[_dedupeColumn].DataType;
            _removeTable.Columns.Add(_dedupeColumn, dedupeColumnType);
            _removeTable.DefaultView.Sort = _dedupeColumn + " ASC";
        }

        /// <summary>
        /// Sets the rows in the _removeList to take into account fields already run through.
        /// </summary>
        /// <param name="list">The list to add rows from</param>
        private async Task SetRemovedRows(UploadedList list) {
            await Task.Run(() => {
                foreach(DataRow dr in list.ListDetails.Rows) {
                    _removeTable.Rows.Add(dr[_dedupeColumn]);
                }
            });
        }

        #endregion
    }
}
