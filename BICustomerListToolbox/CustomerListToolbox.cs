using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BIApps.ListToolbox {

    class CustomerListToolbox {

        #region Private Fields

        /// <summary>
        /// Execution log details for logging to the database
        /// </summary>
        private ExecutionLogDetails _executionLog;

        /// <summary>
        /// The CustomerListSet inputted from external sources, prior to working on them
        /// </summary>
        private CustomerListSet _inputCls;

        /// <summary>
        /// The CustomerListSet containing the lists outputted after being worked on
        /// </summary>
        private CustomerListSet _outputCls;

        /// <summary>
        /// The default folder the lists get outputted to.
        /// TODO: Get this to be user selected
        /// </summary>
        private string _defaultOutputFolder;

        /// <summary>
        /// The Stored Procedure used to check a list of customers for contactibility
        /// </summary>
        private string _contactableStoredProc = "bi_apps.list_toolbox.list_toolbox_proc";

        #endregion

        #region Public Properties

        /// <summary>
        /// The CustomerListSet inputted from external sources, prior to working on them
        /// </summary>
        public CustomerListSet InputCLS {
            get { return this._inputCls; }
            set { this._inputCls = value; }
        }

        /// <summary>
        /// The CustomerListSet containing the lists outputted after being worked on
        /// </summary>
        public CustomerListSet OutputCLS {
            get { return this._outputCls; }
            set { this._outputCls = value; }
        }

        /// <summary>
        /// The default output folder the list is saved to
        /// </summary>
        public string DefaultOutputFolder {
            get { return this._defaultOutputFolder; }
            set { this._defaultOutputFolder = value; }
        }

        /// <summary>
        /// The execution log used for logging usage
        /// </summary>
        public ExecutionLogDetails ExecutionLog {
            get { return this._executionLog; }
        }

        #endregion

        #region Initializers

        public CustomerListToolbox() {
            this._executionLog = new ExecutionLogDetails();
            this._inputCls = new CustomerListSet();
            this._outputCls = new CustomerListSet();
        }

        #endregion

        #region Upload Lists

        /// <summary>
        /// Upload a list of customers to a CustomerListSet
        /// </summary>
        public void UploadCustomerLists(List<string> listOfCustomers) {

            // Log Start
            _executionLog.LogStart("UploadLists", _inputCls);

            // Upload a list of customers
            AddListsToCustomerListSet(listOfCustomers);

            // Log End
            _executionLog.LogEnd("Upload", _inputCls);
        }

        /// <summary>
        /// Upload a list of files to the CustomerListSet
        /// </summary>
        /// <param name="filenames">The list of files to upload</param>
        private void AddListsToCustomerListSet(List<string> filenames) {

            // Loop through the files and import them into a CustomerListSet
            foreach (string file in filenames) {
                try {
                    _inputCls.Add(CustomerList.GetCSV(file));
                } catch(DuplicateNameException) {
                    string errFile = Path.GetFileName(file);
                    string errorMessage = "A file with the name " + errFile + " has already \n"
                        + "been loaded. Please rename the file.";
                    _executionLog.LogError(errorMessage);
                    MessageBox.Show(errorMessage);
                } catch(Exception ex) {
                    _executionLog.LogError(ex.Message);
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Save Lists

        /// <summary>
        /// Output all the tables in a CustomerListSet into a folder as a CSV based on
        /// the lists TableName.
        /// </summary>
        public void SaveList(bool keepHeaders, string executionMethod) {
            try {
                foreach(CustomerList cl in _outputCls.Tables) {
                    string fullFileName = string.Empty;
                    fullFileName = _defaultOutputFolder + "\\[out] " + cl.TableName + ".csv";
                    cl.OutputCSV(fullFileName, keepHeaders);
                }
            } catch(Exception ex) {
                // Log Error
                _executionLog.LogError(ex.Message);
                MessageBox.Show(ex.Message);
            } finally {
                // Log End
                _executionLog.LogEnd(executionMethod, _outputCls);
            }
        }

        /// <summary>
        /// Output all the tables in a CustomerListSet into a folder as a CSV based on
        /// a filename supplied by the user.
        /// </summary>
        /// <param name="filename">The filename to save the list as</param>
        public void SaveList(string filename, bool keepHeaders, string executionMethod) {
            try {
                foreach(CustomerList cl in _outputCls.Tables) {
                    string fullFileName = string.Empty;
                    fullFileName = _defaultOutputFolder + "\\[out] " + filename + ".csv";
                    cl.OutputCSV(fullFileName, keepHeaders);
                }
            } catch(Exception ex) {
                // Log Error
                _executionLog.LogError(ex.Message);
                MessageBox.Show(ex.Message);
            } finally {
                // Log End
                _executionLog.LogEnd(executionMethod, _outputCls);
            }
        }

        #endregion

        #region Split Lists

        /// <summary>
        /// Split lists based on parameters inputted by the user
        /// </summary>
        /// <param name="splitMethod">The method to split the lists by</param>
        /// <param name="splitValue">The value to split by if column selection is selected</param>
        /// <param name="rowcount">The rowcount to split by if rowcount is selected</param>
        public void SplitLists(string splitMethod, string selectedColumn, int rowcount) {

            // Split the Lists
            if(PreExecution("SplitLists")) {
                foreach(CustomerList cl in _inputCls.Tables) {
                    try {
                        // Split by column name
                        if(splitMethod == "ByColumn") _outputCls.Merge(cl.SplitTable(selectedColumn));
                        // Split by rowcount
                        else _outputCls.Merge(cl.SplitTable(rowcount));
                    } catch(Exception ex) {
                        // Log Error
                        _executionLog.LogError(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Merge Lists

        /// <summary>
        /// Merge a collection of lists into one list.
        /// </summary>
        public void MergeLists() {

            // Merge the Lists
            if(PreExecution("MergeLists")) {
                try {
                    CustomerListSet tmp = new CustomerListSet();
                    foreach(CustomerList cl in _inputCls.Tables) {
                        tmp.Add(cl.ConvertColumnsToText());
                    }
                    _outputCls.Add(tmp.MergeLists());
                } catch(DataException ex) {
                    string errorMessage = "A listColumn in the imported csv's have different types of\n"
                                + "data, eg. Text and Numbers. Please edit this in the source:\n"
                                + ex.Message;
                    _executionLog.LogError(errorMessage);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                } catch(Exception ex) {
                    // Log Error
                    _executionLog.LogError(ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region DeDuping Lists

        /// <summary>
        /// Dedupe a selection of lists based on a common column selected by the user.
        /// </summary>
        /// <param name="selection">The column selection to dedupe by</param>
        /// <param name="caseSensitive">Whether the deduping is case sensitive or not</param>
        public void DeDupeLists(string selection, bool caseSensitive) {

            // DeDupe the Lists
            if(PreExecution("DeDupeLists")) {
                try {
                    // DeDupe by column name
                    _outputCls = _inputCls.DeDupeCustomerList(selection, caseSensitive); ;
                } catch(Exception ex) {
                    // Log Error
                    _executionLog.LogError(ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region Contactable Check

        /// <summary>
        /// Check a list to see if the customers are contactable or not
        /// </summary>
        /// <param name="parameters">The list of flags to check the DB for</param>
        /// <param name="joinColumns">The columns to join against the DB with (source and target)</param>
        public void ContactableCheck(List<SqlParameterData> parameters, List<string> joinColumns) {

            // Split the Lists
            if(PreExecution("ContactCheck")) {
                foreach(CustomerList cl in _inputCls.Tables) {
                    try {
                        // Split by column name
                        _outputCls.Add(cl.Contactable(_contactableStoredProc, parameters, joinColumns));
                    } catch(Exception ex) {
                        // Log Error
                        _executionLog.LogError(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Column Selection

        /// <summary>
        /// Output only the selected columns.
        /// </summary>
        /// <param name="selection">The columns selected for output</param>
        public void ColumnSelection(List<string> selection) {

            // DeDupe the Lists
            if(PreExecution("ColSelect")) {
                try {
                    // Loop through each of the CustomerLists and add
                    // a new one to the output CLS containing only
                    // the selected columns
                    foreach(CustomerList cl in _inputCls.Tables) {
                        _outputCls.Add(cl.ColumnSelect(selection));
                    }
                } catch(Exception ex) {
                    // Log Error
                    _executionLog.LogError(ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        #endregion

        #region General Private Methods

        /// <summary>
        /// Clear all the data from the Output CustomerListSet
        /// </summary>
        private void ClearOutputLists() {
            _outputCls.Reset();
            _outputCls.AcceptChanges();
        }

        /// <summary>
        /// Clear all the data from the Input CustomerListSet
        /// </summary>
        private void ClearInputLists() {
            _inputCls.Reset();
            _inputCls.AcceptChanges();
        }

        /// <summary>
        /// Clear all the data from the all CustomerListSets
        /// </summary>
        private void clearAllSets() {
            ClearOutputLists();
            ClearInputLists();
        }

        /// <summary>
        /// Ensure there are lists loaded before acting on them.
        /// </summary>
        /// <param name="cls">CustomerListSet to check for lists.</param>
        /// <returns>Boolean value to test against.</returns>
        private bool CheckListsAreLoaded(CustomerListSet cls) {
            bool output = true;
            if(cls.Tables.Count == 0) {
                MessageBox.Show("No lists found, please upload a CSV to begin.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                output = false;
            }
            return output;
        }

        private bool PreExecution(string executionMethod) {

            // Log Start
            this._executionLog.LogStart(executionMethod, _inputCls);

            // Clear the lists if there are any already in the output set
            ClearOutputLists();

            // Ensure some input lists are loaded
            return CheckListsAreLoaded(_inputCls);
        }

        #endregion
    }
}
