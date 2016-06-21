using BIApps.DialogService;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class ContactableViewModel : ViewModelBase {
        public ContactableViewModel(IMessenger messenger, ILoading loading, IDialogService dialog) : base(messenger, loading, dialog) { }
    }
}
