namespace BIApps.ListToolbox.ListHelpers.Savers {

    /// <summary>
    /// This interface outlines how to save a particular list given a target name.
    /// </summary>
    public interface IListSaver {
        void SaveList(UploadedList list, string listPath, bool keepHeaders);
        void SaveLists(UploadedListGroup lists, string listPath, bool keepHeaders);
    }
}
