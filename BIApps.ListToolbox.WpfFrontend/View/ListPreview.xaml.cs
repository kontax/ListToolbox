using System.Windows;
using System.Windows.Input;

namespace BIApps.ListToolbox.WpfFrontend.View {
    /// <summary>
    /// Interaction logic for ListPreview.xaml
    /// </summary>
    public partial class ListPreview : Window {
        public ListPreview() {
            InitializeComponent();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void btnMinimizeApp_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void btnCloseApp_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
