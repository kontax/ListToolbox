using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace BIApps.ListToolbox {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        #region New Version

        #region Private Fields

        /// <summary>
        /// The toolbox class containing the methods that act on the lists
        /// </summary>
        private CustomerListToolbox toolbox = new CustomerListToolbox();

        /// <summary>
        /// The mutex used to ensure there is only one other thread being used to perform
        /// oprations on at any one time.
        /// </summary>
        private SemaphoreSlim _mutex = new SemaphoreSlim(1);

        /// <summary>
        /// The priority list of customer identifiers to check against for automating 
        /// the controls (via explorer context menu).
        /// </summary>
        private ObservableCollection<string> _joinColumnObs = new ObservableCollection<string>();

        #endregion

        #region To replace in the future

        private void UpdateInputObservableCollection() {
            _inputObs.Clear();
            _inputObs = clsToObservableCollection(this.toolbox.InputCLS);
        }

        private void UpdateOutputObservableCollection() {
            _outputObs.Clear();
            _outputObs = clsToObservableCollection(this.toolbox.OutputCLS);
        }


        /// <summary>
        /// Add all the lists in a CustomerListSet to an ObservableCollection to use in the GUI.
        /// </summary>
        /// <param name="cls">CustomerListSet of lists</param>
        /// <returns>OvervableCollection of lists</returns>
        private ObservableCollection<CustomerList> clsToObservableCollection(CustomerListSet cls) {
            var output = new ObservableCollection<CustomerList>();
            foreach(CustomerList cl in cls.Tables) {
                output.Add(cl);
            }
            return output;
        }

        #endregion

        #region Window item click methods

        // Minimize the window
        private void btnMinimizeApp_Click(object sender, RoutedEventArgs e) {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        // Close the window
        private void btnCloseApp_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        // Move the window
        private void MoveWindow(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        // Remove the text in the rowcount textbox once it's been clicked on.
        private void txtSplitRowCountSelection_GotFocus(object sender, RoutedEventArgs e) {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= txtSplitRowCountSelection_GotFocus;
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e) {
            var helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        #endregion

        #region Status Messages

        // Update the status box with a new message
        private void UpdateStatusMessage(string message, bool completed = false) {
            string origText = txtStatusNotification.Text;
            string errorMsg = message;

            if(completed) {
                txtStatusNotification.Text = origText + message;
                scrStatusScrollViewer.ScrollToBottom();
            } else if(origText.Equals(String.Empty)) {
                txtStatusNotification.Text = _statusMessageCounter + ". " + errorMsg;
                _statusMessageCounter++;
                scrStatusScrollViewer.ScrollToBottom();
            } else {
                txtStatusNotification.Text = origText + "\n" + _statusMessageCounter + ". " + errorMsg;
                _statusMessageCounter++;
                scrStatusScrollViewer.ScrollToBottom();
            }
        }

        #endregion

        #region Methods updating GUI during async

        /// <summary>
        /// Method used to start the progress bar and disable all the other
        /// buttons that call heavy methods.
        /// </summary>
        private void StartApplicationBusy() {
            // Start the progress bar
            pbrProgressBar.IsIndeterminate = true;
            // Disable the buttons
            btnClearLists.IsEnabled = false;
            btnContactLists.IsEnabled = false;
            btnDedupeLists.IsEnabled = false;
            btnMergeLists.IsEnabled = false;
            btnSplitLists.IsEnabled = false;
            btnUploadList.IsEnabled = false;
            btnColSelect.IsEnabled = false;
        }


        /// <summary>
        /// Method used to end the progress bar and renable all the other
        /// buttons that call heavy methods.
        /// </summary>
        private void StopApplicationBusy() {
            // Start the progress bar
            pbrProgressBar.IsIndeterminate = false;
            // Disable the buttons
            btnClearLists.IsEnabled = true;
            btnContactLists.IsEnabled = true;
            btnDedupeLists.IsEnabled = true;
            btnMergeLists.IsEnabled = true;
            btnSplitLists.IsEnabled = true;
            btnUploadList.IsEnabled = true;
            btnColSelect.IsEnabled = true;
            // Add a message to the status window
            UpdateStatusMessage("done", true);
        }

        #endregion

        #region Upload Lists

        private void btnUploadList_Click(object sender, RoutedEventArgs e) {
            List<string> list = GetCustomerListsFromDialog();
            UploadList(list);
        }

        /// <summary>
        /// Load lists into the application from files
        /// </summary>
        /// <param name="list">A list of files to upload</param>
        public async void UploadList(List<string> list) {

            await _mutex.WaitAsync();
            try {
                var Upload = Task.Run(() => this.toolbox.UploadCustomerLists(list));
                StartApplicationBusy();
                UpdateStatusMessage("Uploading lists...");
                await Upload;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Clear the ObservableCollection and readd all the tables
            UpdateInputObservableCollection();
            lbxLoadedLists.DataContext = _inputObs;

            // Update the GUI to show the uploaded filenames and columns in common
            _colsInCommon = new ObservableCollection<string>(this.toolbox.InputCLS.ColumnsInCommon());
            cmbSplitColumnSelection.DataContext = _colsInCommon;
            cmbDedupeColumnSelection.DataContext = _colsInCommon;
            cmbContactColumnSelection.DataContext = _colsInCommon;
            lbxColSelectColumnSelection.DataContext = _colsInCommon;
            txtSaveLocation.Text = this.toolbox.DefaultOutputFolder;
            txtSaveLocation.ToolTip = this.toolbox.DefaultOutputFolder;
        }

        /// <summary>
        /// Pop up a dialog box so the user can get a list of filenames to upload
        /// </summary>
        /// <returns>A list containing filenames of files to upload</returns>
        private List<string> GetCustomerListsFromDialog() {

            List<string> output = new List<string>();

            // Get a new file dialog
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "Customer Lists (*.csv,*.txt,*.tab,*.xlsx,*.xls)|*.csv;*.txt;*.tab;*.xlsx;*.xls";
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();

            if((bool)result) {
                // Sets the default save location as the upload folder location.
                this.toolbox.DefaultOutputFolder = System.IO.Path.GetDirectoryName(dlg.FileName);

                // Loop through all selected files and add them to the list of filenames.
                foreach(string filename in dlg.FileNames) {
                    output.Add(filename);
                }
            }

            return output;
        }

        #endregion

        #region Split Lists

        /// <summary>
        /// Event used to call the SplitLists method.
        /// </summary>
        private void btnSplitLists_Click(object sender, RoutedEventArgs e) {

            if((bool)rdoSplitColumns.IsChecked) {
                SplitLists("ByColumn");
            } else if((bool)rdoSplitRowsHeaders.IsChecked) {
                SplitLists("ByRowsWithHeader");
            } else if((bool)rdoSplitRowsNoHeaders.IsChecked) {
                SplitLists("ByRowsNoHeader");
            } else {
                MessageBox.Show("Please select a method to split by.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Split lists based on a user inputted value
        /// </summary>
        /// <param name="splitMethod">The method used to split the lists by</param>
        private async void SplitLists(string splitMethod) {

            // Get the rows to split by (if applicable)
            int rowcount = 0;

            // Get the column to split by or the rowcount
            string selection = splitMethod == "ByColumn" ? cmbSplitColumnSelection.SelectedItem.ToString() : String.Empty;

            // Set the boolean value of whether to keep headers or not
            bool withHeaders = splitMethod == "ByRowsNoHeader" ? false : true;

            // Check the valid selections have been made
            if(splitMethod == "ByColumn") {
                if(cmbSplitColumnSelection.SelectedItem == null) {
                    MessageBox.Show("Please select a listColumn to split by.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            } else {
                if(!int.TryParse(txtSplitRowCountSelection.Text, out rowcount)) {
                    MessageBox.Show("Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Split lists asynchronously
            await _mutex.WaitAsync();
            try {
                var SplitLists = Task.Factory.StartNew(() => this.toolbox.SplitLists(splitMethod, selection, rowcount));
                StartApplicationBusy();
                UpdateStatusMessage("Splitting lists...");
                await SplitLists;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Remove in future
            UpdateOutputObservableCollection();
            lbxSplitOutput.DataContext = _outputObs;

            // Save the files as CSV
            await _mutex.WaitAsync();
            try {
                var SaveFile = Task.Factory.StartNew(() => this.toolbox.SaveList(withHeaders, splitMethod));
                StartApplicationBusy();
                UpdateStatusMessage("Saving files...");
                await SaveFile;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }
        }

        #endregion

        #region Merge Lists

        /// <summary>
        /// Event used to call the Merge method.
        /// </summary>
        private void btnMergeLists_Click(object sender, RoutedEventArgs e) {
            MergeLists();
        }

        /// <summary>
        /// Merge multiple lists together
        /// </summary>
        private async void MergeLists() {

            // Merge all input lists and save to the output CustomerListSet
            await _mutex.WaitAsync();
            try {
                var MergeLists = Task.Factory.StartNew(() => this.toolbox.MergeLists());
                StartApplicationBusy();
                UpdateStatusMessage("Merging lists...");
                await MergeLists;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Remove in future
            UpdateOutputObservableCollection();
            lbxMergeOutput.DataContext = _outputObs;

            // Save the files as CSV
            await _mutex.WaitAsync();
            try {
                var SaveFiles = Task.Factory.StartNew(() => this.toolbox.SaveList("Merged List", true, "NA"));
                StartApplicationBusy();
                UpdateStatusMessage("Saving files...");
                await SaveFiles;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }
        }

        #endregion

        #region Dedupe Lists

        /// <summary>
        /// Event used to call the Dedupe method.
        /// </summary>
        private void btnDedupeLists_Click(object sender, RoutedEventArgs e) {
            DeDupeLists();
        }

        /// <summary>
        /// Dedupe lists based on a column
        /// </summary>
        private async void DeDupeLists() {

            // Choice of the column to dedupe by, and whether it's case sensitive or not
            string selection = cmbDedupeColumnSelection.SelectedItem.ToString();
            bool caseSensitive = (bool)chkDedupeCaseSensitive.IsChecked;

            // Dedupe all input lists and save to the output CustomerListSet
            await _mutex.WaitAsync();
            try {
                var DeDupeLists = Task.Factory.StartNew(() => this.toolbox.DeDupeLists(selection, caseSensitive));
                StartApplicationBusy();
                UpdateStatusMessage("DeDuping lists...");
                await DeDupeLists;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Remove in future
            UpdateOutputObservableCollection();
            lbxDedupeOutput.DataContext = _outputObs;

            // Save the files as CSV
            await _mutex.WaitAsync();
            try {
                var SaveFiles = Task.Factory.StartNew(() => this.toolbox.SaveList(true, "NA"));
                StartApplicationBusy();
                UpdateStatusMessage("Saving files...");
                await SaveFiles;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }
        }

        #endregion

        #region Contactable Check

        /// <summary>
        /// Event used to call the Contact Check method.
        /// </summary>
        private void btnContactable_Click(object sender, RoutedEventArgs e) {
            ContactCheck();
        }

        /// <summary>
        /// Check for customer contactibility
        /// </summary>
        private async void ContactCheck() {

            // Get the proc parameters
            List<SqlParameterData> parameters = GetProcParameters();

            // Get the type of contact check being pulled
            string contactMethod = (from p in parameters
                                    where p.Name == "@RETURN_FLAG"
                                    select p.Value).SingleOrDefault().ToString();

            // Get the columns to join on
            List<string> joinColumns = GetJoinColumns();

            // Run the contactable methods
            await _mutex.WaitAsync();
            try {
                var ContactCheck = Task.Factory.StartNew(() => this.toolbox.ContactableCheck(parameters, joinColumns));
                StartApplicationBusy();
                UpdateStatusMessage("Checking for contact status...");
                await ContactCheck;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Remove in future
            UpdateOutputObservableCollection();
            lbxContactOutput.DataContext = _outputObs;

            // Save the files as CSV
            await _mutex.WaitAsync();
            try {
                var SaveFiles = Task.Factory.StartNew(() => this.toolbox.SaveList(true, contactMethod));
                StartApplicationBusy();
                UpdateStatusMessage("Saving files...");
                await SaveFiles;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Get the list of parameters used to input into the Contact Check stored procedure
        /// </summary>
        /// <returns></returns>
        private List<SqlParameterData> GetProcParameters() {

            // Stored Proc parameters
            List<SqlParameterData> parameters = new List<SqlParameterData>();

            // Pull the selected checkboxes in the GUI.
            string returnFlagChecked;
            if((bool)rdoContactOutputRemove.IsChecked)
                returnFlagChecked = "Remove";
            else
                returnFlagChecked = "Flag";

            string andOrFlagChecked;
            if((bool)rdoContactAnd.IsChecked)
                andOrFlagChecked = "AND";
            else
                andOrFlagChecked = "OR";

            string joinColumn = cmbContactJoinColumn.Text;
            string listColumn = cmbContactColumnSelection.Text;

            // Add all the selections to the parameter list
            parameters.Add(new SqlParameterData("@QUERY_ID", SqlDbType.Int, 1));
            parameters.Add(new SqlParameterData("@SCHEMA_NAME", SqlDbType.Text, cmbContactSchema.Text));
            parameters.Add(new SqlParameterData("@RETURN_FLAG", SqlDbType.Text, returnFlagChecked));
            parameters.Add(new SqlParameterData("@JOIN_COLUMN", SqlDbType.Text, joinColumn));
            parameters.Add(new SqlParameterData("@LIST_COLUMN", SqlDbType.Text, listColumn));
            parameters.Add(new SqlParameterData("@EMAIL", SqlDbType.Bit, chkContactEmail.IsChecked));
            parameters.Add(new SqlParameterData("@PHONE", SqlDbType.Bit, chkContactPhone.IsChecked));
            parameters.Add(new SqlParameterData("@SMS", SqlDbType.Bit, chkContactSms.IsChecked));
            parameters.Add(new SqlParameterData("@POST", SqlDbType.Bit, chkContactPost.IsChecked));
            parameters.Add(new SqlParameterData("@AND_OR", SqlDbType.Text, andOrFlagChecked));

            return parameters;
        }

        /// <summary>
        /// Get a list of valid columns to join against from the list
        /// </summary>
        /// <returns></returns>
        private List<string> GetJoinColumns() {

            // List of valid selections to join on
            List<string> joinColumns = new List<string>();

            foreach(String joinCol in cmbContactJoinColumn.Items) {
                joinColumns.Add(joinCol);
            }

            return joinColumns;
        }

        #endregion

        #region Column Select

        /// <summary>
        /// Event used to call the Column Select method.
        /// </summary>
        private void btnColSelect_Click(object sender, RoutedEventArgs e) {
            ColSelect();
        }

        /// <summary>
        /// Output the selected columns to a new list
        /// </summary>
        private async void ColSelect() {

            // Choice of the columns to output
            var selection = new List<string>();
            foreach(var item in lbxColSelectColumnSelection.SelectedItems) {
                selection.Add(item.ToString());
            }

            // Dedupe all input lists and save to the output CustomerListSet
            await _mutex.WaitAsync();
            try {
                var ColumnSelect = Task.Factory.StartNew(() => this.toolbox.ColumnSelection(selection));
                StartApplicationBusy();
                UpdateStatusMessage("Outputting columns...");
                await ColumnSelect;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }

            // Remove in future
            UpdateOutputObservableCollection();
            lbxColSelectOutput.DataContext = _outputObs;

            // Save the files as CSV
            await _mutex.WaitAsync();
            try {
                var SaveFiles = Task.Factory.StartNew(() => this.toolbox.SaveList(true, "NA"));
                StartApplicationBusy();
                UpdateStatusMessage("Saving files...");
                await SaveFiles;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
            }
        }

        #endregion

        #region InputList Priority change events

        /// <summary>
        /// Event used to drag/drop lists for deduping priority in the output listbox.
        /// </summary>
        private void lbxLoadedLists_PreviewMouseMove(object sender, MouseButtonEventArgs e) {

            // If the user double clicks, pass this on to the DoubleClick event.
            // This was the only way it would work with both, as using only one caused
            // the sender to be undefined.
            if(e.ClickCount > 1) ListBoxItem_MouseDoubleClick(sender, e);

            // If the user is holding down the left mouse button
            else if(sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed) {

                // The listbox item being moved
                ListBoxItem draggedItem = sender as ListBoxItem;

                // Call the drop event method
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);

                draggedItem.IsSelected = true;

                // Update the priority of all the lists in the listbox
                foreach(CustomerList cl in lbxLoadedLists.Items) {
                    cl.Priority = lbxLoadedLists.Items.IndexOf(cl);
                }
            }
        }

        /// <summary>
        /// Event used to catch when a user drag/drops a list in the listboxes.
        /// </summary>
        private void lbxLoadedLists_Drop(object sender, DragEventArgs e) {

            // The list being dropped into place
            CustomerList droppedData = e.Data.GetData(typeof(CustomerList)) as CustomerList;

            // The switch target
            CustomerList target = ((ListBoxItem)(sender)).DataContext as CustomerList;

            // Sort out the indexes of each list
            int removedIdx = lbxLoadedLists.Items.IndexOf(droppedData);
            int targetIdx = lbxLoadedLists.Items.IndexOf(target);

            // Switch them based on their index
            if(removedIdx < targetIdx) {
                _inputObs.Insert(targetIdx + 1, droppedData);
                _inputObs.RemoveAt(removedIdx);
            } else {
                int remIdx = removedIdx + 1;
                if(_inputObs.Count + 1 > remIdx) {
                    _inputObs.Insert(targetIdx, droppedData);
                    _inputObs.RemoveAt(remIdx);
                }
            }
        }

        /// <summary>
        /// Event to catch double clicks on lists within listboxes. This opens a new window
        /// displaying the contents of the list.
        /// </summary>
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            ListBoxItem lbi = (ListBoxItem)sender;
            CustomerList loadedList = (CustomerList)lbi.DataContext;
            BIApps.ListToolbox.ListPreview prv = new BIApps.ListToolbox.ListPreview(loadedList);
            prv.Show();
        }

        #endregion

        #region Other Instances/Context Menu Methods

        /// <summary>
        /// Get the command line args passed from another instance of the application
        /// </summary>
        /// <param name="args">The arguments supplied by the user</param>
        public void ProcessCommandLineArgs(IList<string> args) {

            // Check to see if args have been passed
            if(args.Count > 1) {

                string executionMethod = GetArgsFlags(args, "-m");
                List<string> filenames = GetArgsFiles(args);
                string column = GetArgsFlags(args, "-h");
                string contactMethod = GetArgsFlags(args, "-c");

                // Clear and Upload the lists
                clearAllSets();
                UploadList(filenames);

                // Run through the execution method passed and choose what to do
                switch(executionMethod) {
                    case "upload":
                        break;
                    case "split_by_column":
                        SetCorrectFlags(executionMethod, column);
                        break;
                    case "split_by_rowcount":
                        SetCorrectFlags(executionMethod, null);
                        ContextMenuSplitRows();
                        break;
                    case "merge":
                        tabMergeLists.IsSelected = true;
                        MergeLists();
                        break;
                    case "dedupe":
                        tabDeDupeLists.IsSelected = true;
                        SetCorrectFlags(executionMethod, column);
                        break;
                    case "contact_remove":
                        tabContactableLists.IsSelected = true;
                        SetCorrectFlags(executionMethod, contactMethod);
                        break;
                    case "contact_flag":
                        tabContactableLists.IsSelected = true;
                        SetCorrectFlags(executionMethod, contactMethod);
                        break;
                    default:
                        ContextMenuInvalidSelection(executionMethod, filenames);
                        break;
                }
            }
        }

        /// <summary>
        /// Process the command line arguments to return the method to use on the lists
        /// </summary>
        private string GetArgsFlags(IList<string> args, string flag) {
            var method = args.SingleOrDefault(arg => arg.StartsWith(flag));
            if(!string.IsNullOrEmpty(method)) {
                method = method.Replace(flag + " ", "");
            }
            return method;
        }

        /// <summary>
        /// Process the command line arguments to return the list of files to upload
        /// </summary>
        private List<string> GetArgsFiles(IList<string> args) {
            List<string> files = new List<string>();
            foreach(string filename in args) {
                if(filename.StartsWith("-f")) {
                    string file = filename.Replace("-f ", "");
                    files.Add(System.IO.Path.GetFullPath(file));
                    this.toolbox.DefaultOutputFolder = System.IO.Path.GetDirectoryName(file);
                }
            }
            return files;
        }

        /// <summary>
        /// Point a customer to the Split Lists tab (as they need to choose a column or rowcount to split by).
        /// </summary>
        private void ContextMenuSplitRows() {
            MessageBox.Show("Please select a split method", "Note", MessageBoxButton.OK, MessageBoxImage.Information);
            this.tabSplitLists.IsSelected = true;
        }

        /// <summary>
        /// Set the correct flags after getting the column to check against
        /// </summary>
        /// <param name="method"></param>
        private async void SetCorrectFlags(string method, string column) {

            if(await CheckBestColumnSelection(method)) {
                switch(method) {
                    case "split_by_column":
                        this.rdoSplitColumns.IsChecked = true;
                        this.cmbSplitColumnSelection.SelectedIndex = this.cmbSplitColumnSelection.Items.IndexOf(column);
                        SplitLists("ByColumn");
                        break;
                    case "split_by_rows":
                        this.rdoSplitRowsHeaders.IsChecked = true;
                        break;
                    case "dedupe":
                        this.cmbDedupeColumnSelection.SelectedIndex = this.cmbDedupeColumnSelection.Items.IndexOf(column);
                        DeDupeLists();
                        break;
                    case "contact_remove":
                        this.rdoContactOutputRemove.IsChecked = true;
                        this.rdoContactOutputFlag.IsChecked = false;
                        SetContactMethodFlags(column);
                        ContactCheck();
                        break;
                    case "contact_flag":
                        this.rdoContactOutputRemove.IsChecked = false;
                        this.rdoContactOutputFlag.IsChecked = true;
                        SetContactMethodFlags(column);
                        ContactCheck();
                        break;
                }

            } else {
                MessageBox.Show("The application cannot find an appropriate column in the list \n" +
                                "provided to check contact status against the database with. Please \n" +
                                "check the list and ensure it contains a customer reference column, \n" +
                                "eg. Cust ID, Username, Email Address etc. If you feel that a column \n" +
                                "needs to be added, please contact BI Apps.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Set the contact method flag to check against when doing the contact check method
        /// </summary>
        /// <param name="column">The contact method passed as a command line parameter</param>
        private void SetContactMethodFlags(string column) {
            switch(column) {
                case "email":
                    this.chkContactEmail.IsChecked = true;
                    this.chkContactSms.IsChecked = false;
                    this.chkContactPhone.IsChecked = false;
                    this.chkContactPost.IsChecked = false;
                    break;
                case "sms":
                    this.chkContactEmail.IsChecked = false;
                    this.chkContactSms.IsChecked = true;
                    this.chkContactPhone.IsChecked = false;
                    this.chkContactPost.IsChecked = false;
                    break;
                case "phone":
                    this.chkContactEmail.IsChecked = false;
                    this.chkContactSms.IsChecked = false;
                    this.chkContactPhone.IsChecked = true;
                    this.chkContactPost.IsChecked = false;
                    break;
                case "post":
                    this.chkContactEmail.IsChecked = false;
                    this.chkContactSms.IsChecked = false;
                    this.chkContactPhone.IsChecked = false;
                    this.chkContactPost.IsChecked = true;
                    break;
            }
        }

        /// <summary>
        /// Get the column to run the checks against, using the priority list specified in the fields.
        /// </summary>
        private async Task<bool> CheckBestColumnSelection(string method) {

            bool foundColumn = false;
            string listColumn = string.Empty;
            string joinColumn = string.Empty;

            // Use an async method using the mutex as we want the lists to finish uploading
            // before performing any functionality on them.
            await _mutex.WaitAsync();
            try {
                var CheckSelection = Task.Run<bool>(() => {

                    // Get the column all the lists have in column to check against
                    List<string> columnsInCommon = this.toolbox.InputCLS.ColumnsInCommon();

                    // Ensure the column names are fixed so they can be correctly found
                    var lowercolumns = (from s in columnsInCommon
                                       select s.ToLower().Replace(' ', '_')).ToList<string>();

                    // Check the columns in the lists and the priority identifiers and 
                    // return those values if a match is found
                    foreach(string pc in _joinColumnObs) {
                        foreach(string cc in lowercolumns) {
                            // Take care of the special case where ppo_cust_id is the same as cust_id
                            // and email_address means the same as email
                            string compvalue = cc;
                            if(cc == "cust_id") compvalue = "ppo_cust_id";
                            if(cc == "email_address") compvalue = "email";
                            if(compvalue == pc) {
                                // Incase the columns were edited, we need to get the original
                                // column name to check against here.
                                listColumn = columnsInCommon[lowercolumns.IndexOf(cc)];
                                joinColumn = pc;
                                return true;
                            }
                        }
                    }
                    return false;
                });
                StartApplicationBusy();
                foundColumn = await CheckSelection;
                StopApplicationBusy();
            } finally {
                _mutex.Release();
                switch(method) {
                    case "dedupe":
                        this.cmbDedupeColumnSelection.SelectedIndex = this.cmbDedupeColumnSelection.Items.IndexOf(listColumn);
                        break;
                    case "split_by_column":
                        break;
                    case "contact_remove":
                    case "contact_flag":
                        this.cmbContactColumnSelection.SelectedIndex = this.cmbContactColumnSelection.Items.IndexOf(listColumn);
                        this.cmbContactJoinColumn.SelectedIndex = this.cmbContactJoinColumn.Items.IndexOf(joinColumn);
                        break;
                }
            }

            return foundColumn;
        }

        /// <summary>
        /// Method called when an invalid selection is passed into the contact methods (shouldn't happen)
        /// </summary>
        private void ContextMenuInvalidSelection(string executionMethod, List<string> filenames) {
            this.toolbox.ExecutionLog.LogStart(executionMethod, this.toolbox.InputCLS);
            this.toolbox.ExecutionLog.LogError("Invalid context menu selection");
            this.toolbox.ExecutionLog.LogEnd("NA", this.toolbox.InputCLS);
        }


        #endregion

        #endregion

        #region Old Version (delete as soon as INotify... has been added to CLS, and rest been replaced)

        #region Class properties

        // ObservableCollections used to bind to the objects in the GUI
        private ObservableCollection<CustomerList> _inputObs = new ObservableCollection<CustomerList>();
        private ObservableCollection<CustomerList> _outputObs = new ObservableCollection<CustomerList>();
        private ObservableCollection<string> _colsInCommon = new ObservableCollection<string>();

        private string _loginDetails = Convert.ToString(System.Security.Principal.WindowsIdentity.GetCurrent().Name);

        // The schema's available to the logged in user
        private ObservableCollection<string> _schemaObs = new ObservableCollection<string>();

        // The columns the user can join customers to the DB on

        // Status message counter
        private int _statusMessageCounter = 1;

        // Commandline arguments
        string[] args = Environment.GetCommandLineArgs();

        #endregion

        #region Main window initializer

        public MainWindow() {
            InitializeComponent();

            // Log Start
            this.toolbox.ExecutionLog.LogStart("Main", this.toolbox.InputCLS);

            // Pull the details of the schema's available to the logged in user
            foreach(DataRow row in DBCommon.getUserSchemas().Rows) {
                string obj = (string)row["value"];
                _schemaObs.Add(obj);
            }

            // Get the join columns for the contactable section
            foreach(DataRow row in DBCommon.GetCustomerJoinColumns().Rows) {
                string obj = (string)row["value"];
                _joinColumnObs.Add(obj);
            }

            // Data Contexts
            txtLoginDetails.DataContext = _loginDetails;
            //lbxLoadedLists.DataContext = _inputObs;
            cmbContactSchema.DataContext = _schemaObs;
            cmbContactJoinColumn.DataContext = _joinColumnObs;
            txtStatusNotification.Text = String.Empty;

            // Log End
            this.toolbox.ExecutionLog.LogEnd("OpeningApp", this.toolbox.InputCLS);

            // Process command line arguments
            ProcessCommandLineArgs(this.args);

            // Log on closing
            Closing += MainWindow_Closing;
        }


        /// <summary>
        /// Event called when closing the app.
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.toolbox.ExecutionLog.LogStart("Main", this.toolbox.InputCLS);
            this.toolbox.ExecutionLog.LogEnd("ClosingApp", this.toolbox.OutputCLS);
        }

        #endregion

        #region Methods to remove all input and output lists loaded in the program

        /// <summary>
        /// Clear all the lists already loaded in the application.
        /// </summary>
        private void btnClearLists_Click(object sender, RoutedEventArgs e) {

            // Log Start
            this.toolbox.ExecutionLog.LogStart("UploadLists", this.toolbox.InputCLS);

            // Clear all CustomerListSets loaded
            clearAllSets();
            UpdateStatusMessage("Lists cleared");

            // Log End
            this.toolbox.ExecutionLog.LogEnd("Clear", this.toolbox.InputCLS);
        }

        private void clearOutputSets() {
            this.toolbox.OutputCLS.Reset();
            this.toolbox.OutputCLS.AcceptChanges();
            _outputObs.Clear();
        }

        private void clearInputSets() {
            this.toolbox.InputCLS.Reset();
            this.toolbox.InputCLS.AcceptChanges();
            _inputObs.Clear();
        }

        private void clearAllSets() {
            clearOutputSets();
            clearInputSets();
        }

        #endregion

        #endregion
    }
}
