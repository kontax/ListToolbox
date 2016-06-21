using System.ComponentModel;

namespace BIApps.ListToolbox.WpfFrontend.Helpers {
    public enum Method {
        [Description("upload")] Upload,
        [Description("split_by_column")] SplitByColumn,
        [Description("split_by_rowcount")] SplitByRowcount,
        [Description("merge")] Merge,
        [Description("dedupe")] DeDupe
    }
}
