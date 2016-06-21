using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace BIApps.ListToolbox.WpfFrontend {
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window {

        public HelpWindow() {
            InitializeComponent();
        }

        #region Window item click methods

        // Minimize the window
        private void btnMinimizeApp_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        // Close the window
        private void btnCloseApp_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        // Move the window
        private void MoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }


        #endregion
    }
}
