using System.Collections.Generic;
using System.IO;
using System.Linq;
using BIApps.DialogService;
using BIApps.ListToolbox.ListHelpers;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {
    public class MainViewModel : ViewModelBase {
        private readonly CommandLineProcessor _clProcessor;
        private string _saveLocation;

        public string SaveLocation {
            get { return _saveLocation; }
            set {
                if(value == _saveLocation) return;
                _saveLocation = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IMessenger messenger, ILoading loading, IDialogService dialog, CommandLineProcessor clProcessor)
            : base(messenger, loading, dialog) {
            _clProcessor = clProcessor;

            Messenger.Register<UploadedListGroup>(this, "Uploaded", SetSaveLocation);
        }

        public void ProcessCommandLineArgs(IList<string> args) {
            Messenger.Send(_clProcessor.Process(args), "Upload");
        }

        private void SetSaveLocation(UploadedListGroup ulg) {
            SaveLocation = ulg.FilePath;
        }
    }
}