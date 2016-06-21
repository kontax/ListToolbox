namespace BIApps.ListToolbox.WpfFrontend.Helpers {
    public class ListColumn {

        public string ColumnName { get; set; }

        public bool IsSelected { get; set; }

        public ListColumn(string columnName) {
            ColumnName = columnName;
            IsSelected = false;
        }

        public override string ToString() {
            return ColumnName;
        }
    }
}
