using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BIApps.ListToolbox {
    /// <summary>
    /// Interaction logic for ListPreview.xaml
    /// </summary>
    public partial class ListPreview : Window {

        /// <summary>
        /// The list to display in the window
        /// </summary>
        private CustomerList List;

        /// <summary>
        /// Private constructor - only a CustomerList can be added to the window.
        /// </summary>
        private ListPreview() { }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="list">The CustomerList to display in the window</param>
        public ListPreview(CustomerList list) {
            InitializeComponent();
            this.List = list;
            this.dgrListPreview.DataContext = List.DefaultView;
        }

        #region Window item click methods

        // Minimize the window
        private void btnMinimizeApp_Click(object sender, RoutedEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        // Close the window
        private void btnCloseApp_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        // Move the window
        private void MoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        #endregion
    }
}
