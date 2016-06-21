using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BIApps.DialogService;
using BIApps.ListToolbox.ListHelpers;
using BIApps.ListToolbox.ListHelpers.Operators;
using BIApps.ListToolbox.ListHelpers.Savers;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class MergeViewModel : ViewModelBase {

        private readonly IListSaver _saver;
        private ICommand _mergeListsCommand;
        private UploadedListGroup _uploadedLists;

        /// <summary>
        /// The <see cref="UploadedList"/> collection that the user has selected to perform operations on.
        /// </summary>
        public UploadedListGroup UploadedLists {
            get { return _uploadedLists; }
            private set {
                if(Equals(value, _uploadedLists)) return;
                _uploadedLists = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The command used to merge the lists based on the selections made by the user.
        /// </summary>
        public ICommand MergeListsCommand {
            get {
                return _mergeListsCommand ?? (_mergeListsCommand = new RelayCommand(
                    async () => await MergeLists(),
                    CanMergeLists));
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="MergeViewModel"/>, which is the view model used for allowing
        /// the user to merge <see cref="UploadedList"/> objects together.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> used for sending messages to other view models</param>
        /// <param name="loading">The <see cref="ILoading"/> used for notifying users about running tasks</param>
        /// <param name="dialog">The <see cref="IDialogService"/> used to display messages to the user</param>
        /// <param name="saver">The <see cref="IListSaver"/> used to save the results of the operations</param>
        public MergeViewModel(
            IMessenger messenger,
            ILoading loading,
            IDialogService dialog,
            IListSaver saver)
            : base(messenger, loading, dialog) {
            _saver = saver;

            Messenger.Register<UploadedListGroup>(this, "Uploaded", SetUploadedLists);
            Messenger.Register<ProcessedCommand>(this, "Process", async pcmd => await ProcessCommand(pcmd));
        }

        /// <summary>
        /// Process any command line arguments.
        /// </summary>
        /// <param name="pcmd">The <see cref="ProcessedCommand"/> with the methods necessary</param>
        private async Task ProcessCommand(ProcessedCommand pcmd) {
            if(pcmd.Method != Method.Merge) return;
            await MergeLists();
        }

        /// <summary>
        /// Set the lists that the user has uploaded to be in context.
        /// </summary>
        /// <param name="lists">The <see cref="UploadedListGroup"/> containing the lists</param>
        private void SetUploadedLists(UploadedListGroup lists) {
            UploadedLists = lists;
        }

        /// <summary>
        /// Merges the <see cref="UploadedList"/> collection.
        /// </summary>
        private async Task MergeLists() {
            Loading.Start();
            var merger = new Merger(UploadedLists);
            var output = await merger.Operate();
            Messenger.Send(output, "Output");
            _saver.SaveLists(output, output.FilePath, true);
            Loading.End();
        }

        /// <summary>
        /// Whether the user is able to perform an operation on the lists or not.
        /// </summary>
        /// <returns>True if the user can operate, otherwise false</returns>
        private bool CanMergeLists() {
            return !Loading.Value && UploadedLists != null && UploadedLists.Count() >= 2;
        }
    }
}
