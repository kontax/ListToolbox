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
    public class SplitViewModel : ViewModelBase {

        private readonly IListSaver _saver;
        private ICommand _splitListsCommand;
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
        /// Whether the user has selected to split by the column name or not
        /// </summary>
        public bool SplitColumns { get; set; }

        /// <summary>
        /// Whether the user has selected to split by the number of rows without a header row.
        /// </summary>
        public bool SplitRowsHeaders { get; set; }

        /// <summary>
        /// Whether the user has selected to split by the number of rows with a header row.
        /// </summary>
        public bool SplitRowsNoHeaders { get; set; }

        /// <summary>
        /// The number of rows to split the file into when using the row count splitter.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// The column to split by when using the column splitter.
        /// </summary>
        public string SelectedColumn { get; set; }

        /// <summary>
        /// The command used to split the lists based on the selections made by the user.
        /// </summary>
        public ICommand SplitListsCommand {
            get {
                return _splitListsCommand ?? (_splitListsCommand = new RelayCommand(
                    async () => await SplitLists(),
                    CanSplitLists));
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="SplitViewModel"/>, which is the view model used for allowing
        /// the user to split <see cref="UploadedList"/> objects based on column name or row count.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> used for sending messages to other view models</param>
        /// <param name="loading">The <see cref="ILoading"/> used for notifying users about running tasks</param>
        /// <param name="dialog">The <see cref="IDialogService"/> used to display messages to the user</param>
        /// <param name="saver">The <see cref="IListSaver"/> used to save the results of the operations</param>
        public SplitViewModel(
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
            switch(pcmd.Method) {
                case Method.SplitByColumn:
                    SplitColumns = true;
                    SplitRowsNoHeaders = false;
                    SplitRowsHeaders = false;
                    break;
                case Method.SplitByRowcount:
                    SplitColumns = false;
                    SplitRowsNoHeaders = false;
                    SplitRowsHeaders = true;
                    break;
                default:
                    return;
            }
            await SplitLists();
        }

        /// <summary>
        /// Set the lists that the user has uploaded to be in context.
        /// </summary>
        /// <param name="lists">The <see cref="UploadedListGroup"/> containing the lists</param>
        private void SetUploadedLists(UploadedListGroup lists) {
            UploadedLists = lists;
        }

        /// <summary>
        /// Splits the <see cref="UploadedList"/> collection based on the user selections.
        /// </summary>
        private async Task SplitLists() {
            Loading.Start();
            if(SplitColumns) {
                await SplitLists(new ColumnValueSplitter(UploadedLists, SelectedColumn), true);
            }
            if(SplitRowsHeaders) {
                await SplitLists(new RowCountSplitter(UploadedLists, RowCount), true);
            }
            if(SplitRowsNoHeaders) {
                await SplitLists(new RowCountSplitter(UploadedLists, RowCount), false);
            }
            Loading.End();
        }

        /// <summary>
        /// Splits the <see cref="UploadedList"/> collection based on the user selections.
        /// </summary>
        /// <param name="operation">The <see cref="IListOperation"/> used to split the lists</param>
        /// <param name="keepHeaders">Whether to keep the headers of the output list or not</param>
        private async Task SplitLists(IListOperation operation, bool keepHeaders) {
            var output = await operation.Operate();
            Messenger.Send(output, "Output");
            _saver.SaveLists(output, output.FilePath, keepHeaders);
        }

        /// <summary>
        /// Whether the user is able to perform an operation on the lists or not.
        /// </summary>
        /// <returns>True if the user can operate, otherwise false</returns>
        private bool CanSplitLists() {
            return !Loading.Value && UploadedLists != null && UploadedLists.Any();
        }
    }
}
