using System.Data;
using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Uploaders {
    public interface IListUploader {
        string SourceName { get; }
        Task<DataTable> UploadList();
    }
}
