using System.Threading.Tasks;

namespace BIApps.ListToolbox.ListHelpers.Operators {
    public interface IListOperation {
        UploadedListGroup UploadedListGroup { get; }
        Task<UploadedListGroup> Operate();
    }
}
