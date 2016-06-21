using System.Collections.ObjectModel;
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
    public class ColumnSelectViewModel : ViewModelBase {
        
        private readonly IListSaver _saver;
        private ICommand _selectColumnsCommand;
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
        /// The collection of columns that each <see cref="UploadedList"/> has in common.
        /// </summary>
        public ObservableCollection<ListColumn> ColumnsInCommon { get; private set; }

        /// <summary>
        /// The command used to select a subset of columns from the lists specified.
        /// </summary>
        public ICommand SelectColumnsCommand {
            get {
                return _selectColumnsCommand ?? (_selectColumnsCommand = new RelayCommand(
                    async () => await ColumnSelector(),
                    CanSelectColumns));
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="ColumnSelectViewModel"/>, which is the view model used for allowing
        /// the user to select a subset of columns from the uploaded <see cref="UploadedList"/> objects.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> used for sending messages to other view models</param>
        /// <param name="loading">The <see cref="ILoading"/> used for notifying users about running tasks</param>
        /// <param name="dialog">The <see cref="IDialogService"/> used to display messages to the user</param>
        /// <param name="saver">The <see cref="IListSaver"/> used to save the results of the operations</param>
        public ColumnSelectViewModel(IMessenger messenger, ILoading loading, IDialogService dialog, IListSaver saver)
            : base(messenger, loading, dialog) {
            _saver = saver;

            ColumnsInCommon = new ObservableCollection<ListColumn>();

            Messenger.Register<UploadedListGroup>(this, "Uploaded", SetUploadedLists);
        }

        /// <summary>
        /// Set the lists that the user has uploaded to be in context.
        /// </summary>
        /// <param name="lists">The <see cref="UploadedListGroup"/> containing the lists</param>
        private void SetUploadedLists(UploadedListGroup lists) {
            UploadedLists = lists;
            ColumnsInCommon.Clear();
            foreach(var col in lists.ColumnsInCommon) {
                ColumnsInCommon.Add(new ListColumn(col));
            }
        }

        /// <summary>
        /// Selects a subset of columns from the <see cref="UploadedList"/> collection based on selections
        /// made by the user.
        /// </summary>
        private async Task ColumnSelector() {
            Loading.Start();
            var selectedColumns = ColumnsInCommon.Where(c => c.IsSelected).Select(c => c.ColumnName).ToList();
            var selector = new ColumnSelector(UploadedLists, selectedColumns);
            var output = await selector.Operate();
            Messenger.Send(output, "Output");
            _saver.SaveLists(output, output.FilePath, true);
            Loading.End();
        }

        /// <summary>
        /// Whether the user is able to perform an operation on the lists or not.
        /// </summary>
        /// <returns>True if the user can operate, otherwise false</returns>
        private bool CanSelectColumns() {
            return !Loading.Value && UploadedLists != null && UploadedLists.Any();
        }
    }
}
