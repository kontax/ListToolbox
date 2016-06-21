using System.Data;

namespace BIApps.ListToolbox.Model.Uploaders {
    public interface IListUploader {
        string SourceName { get; }
        DataTable UploadList();
    }
}
