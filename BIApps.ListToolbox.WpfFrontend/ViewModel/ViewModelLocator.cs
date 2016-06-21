using BIApps.DialogService;
using BIApps.ListToolbox.ListHelpers.Savers;
using BIApps.ListToolbox.WpfFrontend.Helpers;
using GalaSoft.MvvmLight.Messaging;
using Ninject;

namespace BIApps.ListToolbox.WpfFrontend.ViewModel {

    public class ViewModelLocator {

        private readonly IKernel _kernel;

        public MainViewModel Main {
            get {
                return _kernel.Get<MainViewModel>();
            }
        }

        public ColumnSelectViewModel ColumnSelect {
            get {
                return _kernel.Get<ColumnSelectViewModel>();
            }
        }

        public ContactableViewModel Contactable {
            get {
                return _kernel.Get<ContactableViewModel>();
            }
        }

        public DeDupeViewModel DeDupe {
            get {
                return _kernel.Get<DeDupeViewModel>();
            }
        }

        public LoadedListsViewModel LoadedLists {
            get {
                return _kernel.Get<LoadedListsViewModel>();
            }
        }

        public MergeViewModel Merge {
            get {
                return _kernel.Get<MergeViewModel>();
            }
        }

        public OutputViewModel Output {
            get {
                return _kernel.Get<OutputViewModel>();
            }
        }

        public SplitViewModel Split {
            get {
                return _kernel.Get<SplitViewModel>();
            }
        }

        public ViewModelLocator() {
            _kernel = new StandardKernel();
            _kernel.Bind<IMessenger>().To<Messenger>().InSingletonScope();
            _kernel.Bind<ILoading>().To<Loading>().InSingletonScope();
            _kernel.Bind<IDialogService>().To<MainDialogService>();
            _kernel.Bind<IListSaver>().To<CsvListSaver>();
        }

        public static void Cleanup() {
            // TODO Clear the ViewModels
        }
    }
}