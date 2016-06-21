using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIContextMenu;

namespace BIApps.ListToolbox {

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class BIContextMenu : SharpContextMenu {

        // Fields for getting the image icon
        private Assembly _assembly;
        Stream _iconStream;
        Stream _dedupeStream;
        Stream _mailStream;
        Stream _mergeStream;
        Stream _splitStream;
        Stream _uploadStream;

        // Path to the BI Toolbox application
        string appName = "\\BI List Toolbox\\BIApps.ListToolbox.WpfFrontend.exe";
        string programFolder = "C:\\Program Files";
        string application;

        protected override bool CanShowMenu() {
            // We will always show the menu
            return true;
        }

        protected override ContextMenuStrip CreateMenu() {

            // Create the menu strip
            var menu = new ContextMenuStrip();

            // Get Icon
            try {
                _assembly = Assembly.GetExecutingAssembly();
                _iconStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.icon.png");
                _dedupeStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.DeDupe.png");
                _mailStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.Mail.png");
                _mergeStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.Merge.png");
                _splitStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.Split.png");
                _uploadStream = _assembly.GetManifestResourceStream("BIContextMenu.Resources.Upload.png");
            } catch {
                MessageBox.Show("Error accessing resources");
            }
            Image icon = Image.FromStream(_iconStream);
            Image dedupe = Image.FromStream(_dedupeStream);
            Image mail = Image.FromStream(_mailStream);
            Image merge = Image.FromStream(_mergeStream);
            Image split = Image.FromStream(_splitStream);
            Image upload = Image.FromStream(_uploadStream);

            // Create a BI Apps Submenu
            var itemBIApps = new ToolStripMenuItem { Text = "BI Apps", Image = icon };

            // Add the menu items within the submenu
            ToolStripMenuItem menuItemUpload = new ToolStripMenuItem { Text = "Upload", Image = upload };
            ToolStripMenuItem menuItemSplitByColumn = new ToolStripMenuItem { Text = "Split By Column", Image = split };
            ToolStripMenuItem menuItemSplitByRowCount = new ToolStripMenuItem { Text = "Split By RowCount", Image = split };
            ToolStripMenuItem menuItemMerge = new ToolStripMenuItem { Text = "Merge", Image = merge };
            ToolStripMenuItem menuItemDeDupe = new ToolStripMenuItem { Text = "DeDupe", Image = dedupe };
            ToolStripMenuItem menuItemContactRemove = new ToolStripMenuItem { Text = "Contact Remove", Image = mail };
            ToolStripMenuItem menuItemContactFlag = new ToolStripMenuItem { Text = "Contact Flag", Image = mail };

            // Get the column names for those options that need it
            foreach(string str in FilePreview.GetCommonHeaders(SelectedItemPaths)) {
                menuItemSplitByColumn.DropDownItems.Add(str, null, new EventHandler(SplitListsByColumn));
                menuItemDeDupe.DropDownItems.Add(str, null, new EventHandler(DeDupeLists));
            }

            // Add the options for contact check to the toolstrip items
            menuItemContactRemove.DropDownItems.Add("Email", null, new EventHandler(ContactRemoveEmail));
            menuItemContactRemove.DropDownItems.Add("SMS", null, new EventHandler(ContactRemoveSMS));
            menuItemContactRemove.DropDownItems.Add("Phone", null, new EventHandler(ContactRemovePhone));
            menuItemContactRemove.DropDownItems.Add("Post", null, new EventHandler(ContactRemovePost));

            menuItemContactFlag.DropDownItems.Add("Email", null, new EventHandler(ContactFlagEmail));
            menuItemContactFlag.DropDownItems.Add("SMS", null, new EventHandler(ContactFlagSMS));
            menuItemContactFlag.DropDownItems.Add("Phone", null, new EventHandler(ContactFlagPhone));
            menuItemContactFlag.DropDownItems.Add("Post", null, new EventHandler(ContactFlagPost));

            itemBIApps.DropDownItems.AddRange(new ToolStripItem[] {
                menuItemUpload, 
                menuItemSplitByColumn,
                menuItemSplitByRowCount,
                menuItemMerge,
                menuItemDeDupe,
                menuItemContactRemove,
                menuItemContactFlag
            });

            menu.Items.Add(itemBIApps);

            // Event handlers that get called once we've clicked the items
            menuItemUpload.Click += (sender, args) => UploadLists();
            menuItemSplitByColumn.Click += (sender, args) => NoAction();
            menuItemSplitByRowCount.Click += (sender, args) => SplitListsByRowCount();
            menuItemMerge.Click += (sender, args) => MergeLists();
            menuItemDeDupe.Click += (sender, args) => NoAction();
            menuItemContactRemove.Click += (sender, args) => NoAction();
            menuItemContactFlag.Click += (sender, args) => NoAction();

            // Add the item to the context menu
            menu.Items.Add(itemBIApps);

            // Return the menu
            return menu;
        }

        #region Event handlers

        /// <summary>
        /// Messagebox shown when no common columns can be found.
        /// </summary>
        private void NoAction() {
            MessageBox.Show("No common columns found, please upload the lists first.");
        }

        /// <summary>
        /// Start the process and upload the selected lists to it
        /// </summary>
        private void UploadLists() {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m upload\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and split the selected lists by a specific column
        /// </summary>
        private void SplitListsByColumn(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m split_by_column\" ");
            builder.Append("\"-h " + sender.ToString() + "\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and split the selected lists by a number of rows
        /// </summary>
        private void SplitListsByRowCount() {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m split_by_rowcount\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and Merge the selected lists
        /// </summary>
        private void MergeLists() {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m merge\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and dedupe the selected lists
        /// </summary>
        private void DeDupeLists(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m dedupe\" ");
            builder.Append("\"-h " + sender.ToString() + "\" ");
            StartProcess(builder);
        }

        #endregion

        #region Contact Remove Event Handlers

        /// <summary>
        /// Start the process and remove non-contactable by email customers from the lists
        /// </summary>
        private void ContactRemoveEmail(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_remove\" ");
            builder.Append("\"-c email\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and remove non-contactable by sms customers from the lists
        /// </summary>
        private void ContactRemoveSMS(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_remove\" ");
            builder.Append("\"-c sms\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and remove non-contactable by phone customers from the lists
        /// </summary>
        private void ContactRemovePhone(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_remove\" ");
            builder.Append("\"-c phone\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and remove non-contactable by post customers from the lists
        /// </summary>
        private void ContactRemovePost(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_remove\" ");
            builder.Append("\"-c post\" ");
            StartProcess(builder);
        }

        #endregion

        #region Contact Flag Event Handlers

        /// <summary>
        /// Start the process and flag non-contactable by email customers from the lists
        /// </summary>
        private void ContactFlagEmail(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_flag\" ");
            builder.Append("\"-c email\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and flag non-contactable by sms customers from the lists
        /// </summary>
        private void ContactFlagSMS(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_flag\" ");
            builder.Append("\"-c sms\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and flag non-contactable by phone customers from the lists
        /// </summary>
        private void ContactFlagPhone(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_flag\" ");
            builder.Append("\"-c phone\" ");
            StartProcess(builder);
        }

        /// <summary>
        /// Start the process and flag non-contactable by post customers from the lists
        /// </summary>
        private void ContactFlagPost(object sender, EventArgs e) {
            // Get the filenames to send to the process
            StringBuilder builder = new StringBuilder();
            builder.Append("\"-m contact_flag\" ");
            builder.Append("\"-c post\" ");
            StartProcess(builder);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Start the application with the command line arguments supplied.
        /// </summary>
        /// <param name="commandLineArgs">The list of command line args supplied to the application</param>
        private void StartProcess(StringBuilder commandLineArgs) {

            // Get the location of the installed application
            GetApplicationName();

            //  Go through each file and add them to the stringbuilder
            foreach(var filePath in SelectedItemPaths) {
                commandLineArgs.Append("\"-f " + filePath + "\" ");
            }

            // Start the process with the command line args
            Process toolbox = new Process();
            toolbox.StartInfo.FileName = application;
            toolbox.StartInfo.Arguments = commandLineArgs.ToString();
            
            // Try run the application
            try {
                toolbox.Start();
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Get the location that the application is installed to
        /// </summary>
        private void GetApplicationName() {
            application = programFolder + appName;
            if(File.Exists(application)) {
                return;
            } else {
                programFolder = ProgramFilesx86();
                application = programFolder + appName;
            }
        }

        /// <summary>
        /// Get the program files folder in various system architectures
        /// </summary>
        private string ProgramFilesx86() {
            if(8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432")))) {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        #endregion
    }
}
