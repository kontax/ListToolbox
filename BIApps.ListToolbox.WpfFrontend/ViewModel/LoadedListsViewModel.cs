using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BIApps.DialogService;
using BIApps.ListToolbox.ListHelpers;
using BIApps.ListToolbox.ListHelpers.Uploaders;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class LoadedListsViewModel : ViewModelBase {

        private readonly UploaderFactory _factory;
        private ICommand _uploadListCommand;
        private ICommand _clearListsCommand;

        /// <summary>
        /// The <see cref="UploadedList"/> collection that the user has selected to perform operations on.
        /// </summary>
        public UploadedListGroup UploadedLists { get; set; }

        /// <summary>
        /// The command used to upload lists to perform actions on.
        /// </summary>
        public ICommand UploadListsCommand {
            get {
                return _uploadListCommand ?? (_uploadListCommand = new RelayCommand(
                    async () => await UploadList(),
                    () => !Loading.Value));
            }
        }

        /// <summary>
        /// Clear all <see cref="UploadedList"/> objects from the collection.
        /// </summary>
        public ICommand ClearListsCommand {
            get {
                return _clearListsCommand ?? (_clearListsCommand = new RelayCommand(
                    () => {
                        UploadedLists.Clear();
                        Messenger.Send(UploadedLists, "Uploaded");
                    }));
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="LoadedListsViewModel"/>, which is the view model used for allowing
        /// the user to upload new <see cref="UploadedList"/> objects.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> used for sending messages to other view models</param>
        /// <param name="loading">The <see cref="ILoading"/> used for notifying users about running tasks</param>
        /// <param name="dialog">The <see cref="IDialogService"/> used to display messages to the user</param>
        /// <param name="factory">The <see cref="UploaderFactory"/> used to instantiate <see cref="IListUploader"/> instances</param>
        public LoadedListsViewModel(
            IMessenger messenger,
            ILoading loading,
            IDialogService dialog,
            UploaderFactory factory)
            : base(messenger, loading, dialog) {

            _factory = factory;
            UploadedLists = new UploadedListGroup();
            Messenger.Send(UploadedLists, "Uploaded");

            Messenger.Register<ProcessedCommand>(this, "Upload", async pcl => await ProcessCommandLines(pcl));
        }

        /// <summary>
        /// Processes the command line arguments by uploading the files selected by the user.
        /// </summary>
        /// <param name="pcmd">The <see cref="ProcessedCommand"/></param>
        private async Task ProcessCommandLines(ProcessedCommand pcmd) {
            await Upload(pcmd.Files.ToList());
            Messenger.Send(pcmd, "Process");
        }

        /// <summary>
        /// Asks the user to select a collection of files to upload to the application
        /// </summary>
        private async Task UploadList() {

            var files = GetFiles();
            if(files.Count == 0) return;

            await Upload(files);
        }
        
        /// <summary>
        /// Gets a collection of files selected by the user.
        /// </summary>
        /// <returns>A list of filenames to upload</returns>
        private List<string> GetFiles() {
            var defaultExt = ".csv";
            var options = "Lists (*.csv,*.txt,*.tab,*.xlsx,*.xls)|*.csv;*.txt;*.tab;*.xlsx;*.xls";

            // Get the file selections from the user
            var files = Dialog.OpenFileDialog(defaultExt, options, true).ToList();
            return files;
        }

        /// <summary>
        /// Uploads the list of files to be processed.
        /// </summary>
        /// <param name="files">The list of filenames to upload</param>
        private async Task Upload(List<string> files) {
            var filePath = Path.GetDirectoryName(files.First());
            UploadedLists.FilePath = filePath;

            // Loop through the files and create a new UploadedList for each
            Loading.Start();
            try {
                foreach(var file in files) {
                    var list = await GetFile(file);
                    UploadedLists.Add(list);
                }
            } catch(ArgumentException ex) {
                Dialog.Show(ex.Message);
            } catch(InvalidOperationException ex) {
                Dialog.Show(ex.Message);
            } finally {
                Loading.End();
                Messenger.Send(UploadedLists, "Uploaded");
            }
        }

        /// <summary>
        /// Upload the DataTable for a file and returns the instantiated <see cref="UploadedList"/>.
        /// </summary>
        /// <param name="file">The filename of the file to upload</param>
        /// <returns>A new <see cref="UploadedList"/></returns>
        private async Task<UploadedList> GetFile(string file) {
            var uploader = _factory.GetUploader(file);
            var dataTable = await uploader.UploadList();
            return new UploadedList(dataTable);
        }
    }
}
