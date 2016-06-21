using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using BIApps.ListToolbox.WpfFrontend.ViewModel;

namespace BIApps.ListToolbox.WpfFrontend {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp {

        void app_DispatcherUnhandledException(Object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.Message);
            MessageBox.Show(e.Exception.StackTrace);
            //e.Handled = true;
        }

        private const string Unique = "E47EC37B-C4EF-4814-9794-CBB4487369D1";

        [STAThread]
        public static void Main() {
            if(SingleInstance<App>.InitializeAsFirstInstance(Unique)) {
                var application = new App();
                try {
                    application.InitializeComponent();
                    application.Run();
                } catch(Exception ex) {
                    MessageBox.Show(ex.Message);
                    application.Shutdown();
                }
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members
        public bool SignalExternalCommandLineArgs(IList<string> args) {
            var main = Current.MainWindow.DataContext as MainViewModel;
            main.ProcessCommandLineArgs(args);
            return true;
        }
        #endregion
    }
}
