namespace BIApps.ListToolbox.Model.Savers {

    /// <summary>
    /// This interface outlines how to save a particular list given a target name.
    /// </summary>
    public interface IListSaver {
        string ListName { get; set; }
        string ListPath { get; set; }
        void SaveList();
    }
}
