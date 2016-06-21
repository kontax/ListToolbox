using System.Windows;
using System.Windows.Input;

namespace BIApps.ListToolbox.WpfFrontend.View {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        #region Window item click methods

        // Minimize the window
        private void btnMinimizeApp_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        // Close the window
        private void btnCloseApp_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        // Move the window
        private void MoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e) {
            var helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        #endregion
    }
}
