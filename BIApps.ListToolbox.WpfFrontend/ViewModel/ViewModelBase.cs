using System.ComponentModel;
using System.Runtime.CompilerServices;
using BIApps.DialogService;
using BIApps.ListToolbox.WpfFrontend.Annotations;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class ViewModelBase : INotifyPropertyChanged {

        public IMessenger Messenger { get; private set; }

        public ILoading Loading { get; private set; }

        public IDialogService Dialog { get; set; }

        public ViewModelBase(IMessenger messenger, ILoading loading, IDialogService dialog) {
            Messenger = messenger;
            Loading = loading;
            Dialog = dialog;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}