using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OnlineTimeClockMTSImportFileCreator
{
    public partial class frmMain : Form
    {
        application application;
        access access;
         
        public frmMain()
        {
            InitializeComponent();
            application = new application();
            this.writeInitialEntries();
            this.Text = this.Text + " (Version: " + Application.ProductVersion + ")";
            this.txtDatabasePath.Text = application.DatabasePath;
            this.logMessage("DatabasePath::" + this.txtDatabasePath.Text);
            if (application.ErrorMessage.Length > 0)
            {
                this.logMessage("DatabasePath::" + this.txtDatabasePath.Text);
            }
            this.txtAccount.Text = application.AccountName;
            this.txtAccountAdminLogin.Text = application.AccountLogin;
            this.txtDataUploadToken.Text=string.Empty;
            if (application.ExportPath.Length == 0)
            {
                if (application.bDirectoryExists(System.IO.Path.GetDirectoryName(this.txtDatabasePath.Text)))
                {
                    this.txtExportFileLocation.Text = System.IO.Path.GetDirectoryName(this.txtDatabasePath.Text)+"\\" + "timeclock-import.json";
                }
                else
                {
                    this.txtExportFileLocation.Text = string.Empty;
                }
            }
            else
            {
                this.txtExportFileLocation.Text = application.ExportPath;
            }
            this.logMessage("AccountName::" + this.txtAccount.Text);
            this.logMessage("AccountLogin::" + this.txtAccountAdminLogin.Text);
            this.logMessage("ExportPath::" +this.txtExportFileLocation.Text);           
            this.FormClosing += new FormClosingEventHandler(frmMain_FormClosing);
            this.btnBrowse.Click+=new EventHandler(btnBrowse_Click);
            this.btnBrowseExportLocation.Click+=new EventHandler(btnBrowseExportLocation_Click);
            this.btnClearLog.Click+=new EventHandler(btnClearLog_Click);
            this.btnCopyLog.Click+=new EventHandler(btnCopyLog_Click);
            this.access = new access(this.txtDatabasePath.Text.Trim());
            this.access.LogEvent += new LogEventHandler(logEvent);

#if DEBUG 
            this.chkNoFormValidation.Visible = true;
            this.chkNoValidityChecks.Visible = true;
#else
            this.chkNoFormValidation.Visible = false;
            this.chkNoValidityChecks.Visible = false;            
#endif

        }
        
        /// <summary>
        /// Writes a initial entries to log text box
        /// </summary>

        private void writeInitialEntries()
        {
            this.logMessage(Application.ProductName + " " + Application.ProductVersion);
            System.OperatingSystem osInfo = System.Environment.OSVersion;
            this.logMessage("Operating System:" + osInfo.Platform);
            this.logMessage("OS Version:" + osInfo.Version);
            this.logMessage("OS SP:" + osInfo.ServicePack);
            this.logMessage("OS Version String:" + osInfo.VersionString);
            this.logMessage("Net Assembly Version:" + System.Environment.Version);
            this.logMessage("Computer Name:" + System.Environment.MachineName);
            // writeMessage("64 Bit OS:" + System.Environment.Is64BitOperatingSystem.ToString());
            //writeMessage("64 Bit Process:" + System.Environment.Is64BitProcess.ToString());
        }

        #region data validation
        private bool validateForm()
        {
            //check if an access file is selected
            if (!application.bFileExists(this.txtDatabasePath.Text.Trim()))
            {
                MessageBox.Show("There is no valid Access database file in that location.");
                this.txtDatabasePath.BackColor = Color.LightPink;
                return false;
            }
            this.txtDatabasePath.BackColor = Color.White;
            //check if save export location is set
            if (!application.bDirectoryExists(System.IO.Path.GetDirectoryName(this.txtExportFileLocation.Text.Trim())))
            {
                MessageBox.Show("The directory you are trying to create an import file in does not exist.");
                this.txtExportFileLocation.BackColor = Color.LightPink;
                return false;
            }
            this.txtExportFileLocation.BackColor = Color.White;

            if (this.txtAccount.Text.Trim().Length < 6)
            {
                MessageBox.Show("You must provide a valid Online Time Clock MTS Account Name to create an import file.");
                this.txtAccount.BackColor = Color.LightPink;
                return false;
            }
            this.txtAccount.BackColor = Color.White;

            if (this.txtAccountAdminLogin.Text.Trim().Length < 6)
            {
                MessageBox.Show("You must provide your Online Time Clock MTS account administrator login to create an import file.");
                this.txtAccountAdminLogin.BackColor = Color.LightPink;
                return false;
            }
            this.txtAccountAdminLogin.BackColor = Color.White;

            if (this.txtDataUploadToken.Text.Trim().Length < 6)
            {
                MessageBox.Show("You must provide an Online Time Clock MTS Data Upload Token to create an import file.");
                this.txtDataUploadToken.BackColor = Color.LightPink;
                return false;
            }
            this.txtDataUploadToken.BackColor = Color.White;

            return true;
        }


        #endregion

        #region events
        /// <summary>
        /// Create Export File button click event
        /// </summary>
        private void btnCreateExportFile_Click(object sender, EventArgs e)
        {
            //validiate form data
            if (!this.chkNoFormValidation.Checked)
            {
                this.logMessage("Validating form data");
                if (!this.validateForm())
                {
                    return;
                }
            }
            //offer choice to overwrite existing file
            if (application.bFileExists(this.txtExportFileLocation.Text.Trim()))
            {
                if (MessageBox.Show("An import file exists in that location.  Are you sure you want to overwrite it?", "Confirm Overwrite", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            //check access file is valid
            this.logMessage("Testing if Time Clock Database at " + this.txtDatabasePath.Text.Trim() + " is valid");
            this.access.DatabasePath = this.txtDatabasePath.Text.Trim();
            if (!this.chkNoValidityChecks.Checked)
            {
                if (!this.access.checkValidDatabase(application))
                {
                    MessageBox.Show("Your Time Clock MTS database does not appear to be valid.  Email it to support@timeclockmts.com and we'll check it out for you.");
                    return;
                }
            }
            OnlineTimeClockMTSImportFileCreator.exportFile exportFile = new exportFile(this.access,
                                                                                        this.txtExportFileLocation.Text.Trim(),
                                                                                        this.txtAccount.Text.Trim(),
                                                                                        this.txtAccountAdminLogin.Text.Trim(),
                                                                                        this.txtDataUploadToken.Text.Trim());
            if (exportFile.createExportFile())
            {
                this.logMessage("Online Time Clock MTS Import file created and saved to " + this.txtExportFileLocation.Text);
                MessageBox.Show("Online Time Clock MTS Import file created successfully!");
            }
            else
            {
                this.logMessage("Import file creation failed.  See log for details.");
                MessageBox.Show("Import file creation failed.  See log for details.");
            }

        }
        /// <summary>
        /// Browse for Time Clock MTS database button click event
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string sPath = string.Empty;


            if (this.txtDatabasePath.Text.Length == 0)
            {
                sPath = "c:\\ProgramData\\Time Clock MTS\\Database\\";
            }
            else
            {
                sPath = System.IO.Path.GetDirectoryName(this.txtDatabasePath.Text.Trim());
            }

            this.openFileDialog1.InitialDirectory = sPath;
            this.openFileDialog1.Title = "Find the Time Clock MTS Database";
            this.openFileDialog1.Filter = "Database Files (*.mdb) | *.mdb|All Files (*.*) | *.*";
            this.openFileDialog1.FileName = "";
            this.openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Length == 0)
                {
                    MessageBox.Show("No Time Clock MTS Database Selected", "No Time Clock MTS Database Selected");
                    return;
                }
                sPath = openFileDialog1.FileName;
                if (this.application.bFileExists(sPath))
                {
                    this.txtDatabasePath.Text = sPath;
                }
            }
        }
        /// <summary>
        /// Clears all entries from the log box
        /// </summary>

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the log?", "Confirm Clear Log", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.txtLog.Text = string.Empty;
            }
        }

        /// <summary>
        /// Copies the contents of the log box to the clipboard
        /// </summary>

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtLog.Text);
        }
        /// <summary>
        /// Browse for the export file location button click event
        /// </summary>

        private void btnBrowseExportLocation_Click(object sender, EventArgs e)
        {
            string sPath = string.Empty;


            if (this.txtDatabasePath.Text.Length == 0)
            {
                sPath = "c:\\ProgramData\\Time Clock MTS\\Database\\";
            }
            else
            {
                sPath = System.IO.Path.GetDirectoryName(this.txtDatabasePath.Text.Trim());
            }

            this.saveFileDialog1.InitialDirectory = sPath;
            this.saveFileDialog1.AddExtension = true;
            this.saveFileDialog1.DefaultExt = "json";
            this.saveFileDialog1.Title = "Export File Location";
            this.saveFileDialog1.Filter = "Time Clock MTS Export Files (*.json)|*.json";
            this.saveFileDialog1.FileName = "";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.saveFileDialog1.FileName.Length == 0)
                {
                    MessageBox.Show("No Location Selected to Save Export File", "No Location Selected to Save Export File");
                    return;
                }
                sPath = this.saveFileDialog1.FileName;
                if (this.application.bFileExists(sPath))
                {
                    if (MessageBox.Show("Export file exists, do you want to overwrite it?", "Export File Exists", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                }
                else
                {
                    this.txtExportFileLocation.Text = sPath;
                }
            }
        }

        /// <summary>
        /// Form closing event
        /// </summary>

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            application.saveSettings(this.txtExportFileLocation.Text.Trim(),
                this.txtAccount.Text.Trim(),
                this.txtAccountAdminLogin.Text.Trim(),
                this.txtDatabasePath.Text.Trim());
        }
        /// <summary>
        /// Event bound to access object to allow it to log events to the text box
        /// </summary>
        private void logEvent(object sender, LogEventArgs e)
        {
            if (e.NoNewLine)
                this.logMessage(e.Message, e.NoNewLine);
            else
                this.logMessage(e.Message);
        }
        /// <summary>
        /// Click event for help URL
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.timeclockmts.com/support/exporting-time-clock-mts-data-online-time-clock-mts/");
        }

        #endregion

        #region logging
        /// <summary>
        /// Writes a log message to the log message box overloaded with option to not print a new line
        /// </summary>
        private void logMessage(string pMessage, bool pNoNewLine)
        {
            string sMessage = pMessage;
            sMessage = DateTime.Now.ToString("g") + ": " + sMessage;
            if (sMessage.Substring(sMessage.Length - 1) != System.Environment.NewLine && !pNoNewLine)
            {
                sMessage += System.Environment.NewLine;
            }

            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    string s = string.Empty;
                    if (this.txtLog.Text.Contains(System.Environment.NewLine))
                    {
                        s = this.txtLog.Text.Substring(this.txtLog.Text.IndexOf(System.Environment.NewLine));
                    }
                    else
                        s = this.txtLog.Text;

                    this.txtLog.Text = sMessage + s;
                    this.txtLog.Refresh();
                    Console.Write(sMessage);
                });
            }
            else
            {
                string s = string.Empty;
                if (this.txtLog.Text.Contains(System.Environment.NewLine))
                {
                    s = this.txtLog.Text.Substring(this.txtLog.Text.IndexOf(System.Environment.NewLine));
                }
                else
                    s = this.txtLog.Text;

                this.txtLog.Text = sMessage + s;
                this.txtLog.Refresh();
                Console.Write(sMessage);
            }


            //this.txtLog.Enabled = false;
        }
        /// <summary>
        /// Writes message to log text box.
        /// </summary>
        private void logMessage(string pMessage)
        {
            string sMessage = pMessage;
            sMessage = DateTime.Now.ToString("g") + ": " + sMessage;
            if (sMessage.Substring(sMessage.Length - 1) != System.Environment.NewLine)
            {
                sMessage += System.Environment.NewLine;
            }

            if (this.InvokeRequired)
            {
                this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    this.txtLog.Text = sMessage + this.txtLog.Text;
                    this.txtLog.Refresh();
                    Console.Write(sMessage);
                });
            }
            else
            {
                this.txtLog.Text = sMessage + this.txtLog.Text;
                this.txtLog.Refresh();
                Console.Write(sMessage);
            }


            //this.txtLog.Enabled = false;
        }
        /// <summary>
        /// Clears the log
        /// </summary>
        private void clearLog()
        {
            this.txtLog.Enabled = true;
            this.txtLog.Text = string.Empty;
            //this.txtLog.Enabled = false;
        }
#endregion


    }
}
