using System;
using System.Collections.Generic;
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
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window {

        private string _feedbackProc = "bi_apps.list_toolbox.list_toolbox_proc";

        public HelpWindow() {
            InitializeComponent();
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

        // Send feedback
        private void btnEmailSuggestions_Click(object sender, RoutedEventArgs e) {
            try {
                SendFeedback();
            } catch(Exception ex) {
                string error = "Feedback sending failed!\n" + ex.Message;
                MessageBox.Show(error);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Send the feedback to the feedback proc
        /// </summary>
        private void SendFeedback() {

            // Get the feedback text entered by the user
            string feedback = this.tbxEmailContent.Text;

            // Set up the proc parameters
            List<SqlParameterData> spParams = new List<SqlParameterData>();

            // Add the parameters for the proc
            spParams.Add(new SqlParameterData("@QUERY_ID", System.Data.SqlDbType.Int, 2));
            spParams.Add(new SqlParameterData("@FEEDBACK", System.Data.SqlDbType.VarChar, feedback));

            // Execute the proc
            DBCommon.ExecuteStoredProcedure(_feedbackProc, spParams, ConnectionType.ConnectionStringReadOnly);

            // Show a message and clear the text
            this.tbxEmailContent.Text = "Feedback sent.";
        }

        #endregion
    }
}
