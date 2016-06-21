using BIApps.DialogService;
using BIApps.ListToolbox.ListHelpers;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class OutputViewModel : ViewModelBase {

        private UploadedListGroup _outputLists;

        /// <summary>
        /// The <see cref="UploadedList"/> collection that the user has performed operations on.
        /// </summary>
        public UploadedListGroup OutputLists {
            get { return _outputLists; }
            private set {
                if(Equals(value, _outputLists)) return;
                _outputLists = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="OutputViewModel"/>, which is the view model used for displaying
        /// <see cref="UploadedList"/> objects that have already been operated on.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> used for sending messages to other view models</param>
        /// <param name="loading">The <see cref="ILoading"/> used for notifying users about running tasks</param>
        /// <param name="dialog">The <see cref="IDialogService"/> used to display messages to the user</param>
        public OutputViewModel(IMessenger messenger, ILoading loading, IDialogService dialog)
            : base(messenger, loading, dialog) {

            Messenger.Register<UploadedListGroup>(this, "Output", SetOutputLists);
        }

        /// <summary>
        /// Set the lists that the user has performed operations on to be in context.
        /// </summary>
        /// <param name="lists">The <see cref="UploadedListGroup"/> containing the lists</param>
        private void SetOutputLists(UploadedListGroup lists) {
            OutputLists = lists;
        }
    }
}
