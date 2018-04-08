namespace OnlineTimeClockMTSImportFileCreator
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.txtAccountAdminLogin = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDatabasePath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnCreateExportFile = new System.Windows.Forms.Button();
            this.btnCopyLog = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtDataUploadToken = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtExportFileLocation = new System.Windows.Forms.TextBox();
            this.btnBrowseExportLocation = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkNoFormValidation = new System.Windows.Forms.CheckBox();
            this.chkNoValidityChecks = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtAccountAdminLogin
            // 
            this.txtAccountAdminLogin.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccountAdminLogin.Location = new System.Drawing.Point(186, 132);
            this.txtAccountAdminLogin.Name = "txtAccountAdminLogin";
            this.txtAccountAdminLogin.Size = new System.Drawing.Size(458, 25);
            this.txtAccountAdminLogin.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtAccountAdminLogin, "The login for your Online Time Clock MTS account administrator, the default login" +
                    " is account_admin");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(19, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 17);
            this.label6.TabIndex = 21;
            this.label6.Text = "Account Admin Login";
            // 
            // txtAccount
            // 
            this.txtAccount.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAccount.Location = new System.Drawing.Point(186, 101);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(458, 25);
            this.txtAccount.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtAccount, "Your Online Time Clock MTS account name");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 17);
            this.label4.TabIndex = 19;
            this.label4.Text = "Account";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 21);
            this.label2.TabIndex = 15;
            this.label2.Text = "Online Time Clock MTS Account Details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(276, 21);
            this.label1.TabIndex = 14;
            this.label1.Text = "Time Clock MTS Database Location";
            // 
            // txtDatabasePath
            // 
            this.txtDatabasePath.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDatabasePath.Location = new System.Drawing.Point(22, 38);
            this.txtDatabasePath.Name = "txtDatabasePath";
            this.txtDatabasePath.Size = new System.Drawing.Size(525, 25);
            this.txtDatabasePath.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtDatabasePath, "The path to a current Time Clock MTS database file, typically timeclock.mdb");
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(553, 38);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(91, 25);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.toolTip1.SetToolTip(this.btnBrowse, "Click to locate a current Time Clock MTS database file");
            this.btnBrowse.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(139, 523);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(325, 13);
            this.linkLabel1.TabIndex = 26;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "View Online Help for the Online Time Clock MTS Export File Creator";
            // 
            // btnClearLog
            // 
            this.btnClearLog.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearLog.Location = new System.Drawing.Point(553, 495);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(91, 25);
            this.btnClearLog.TabIndex = 11;
            this.btnClearLog.Text = "Clear Log";
            this.toolTip1.SetToolTip(this.btnClearLog, "Click to clear the log.");
            this.btnClearLog.UseVisualStyleBackColor = true;
            // 
            // btnCreateExportFile
            // 
            this.btnCreateExportFile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreateExportFile.Location = new System.Drawing.Point(22, 272);
            this.btnCreateExportFile.Name = "btnCreateExportFile";
            this.btnCreateExportFile.Size = new System.Drawing.Size(622, 36);
            this.btnCreateExportFile.TabIndex = 8;
            this.btnCreateExportFile.Text = "Create Export File";
            this.toolTip1.SetToolTip(this.btnCreateExportFile, "Click to create the Online Time Clock MTS Export file.");
            this.btnCreateExportFile.UseVisualStyleBackColor = true;
            this.btnCreateExportFile.Click += new System.EventHandler(this.btnCreateExportFile_Click);
            // 
            // btnCopyLog
            // 
            this.btnCopyLog.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyLog.Location = new System.Drawing.Point(553, 328);
            this.btnCopyLog.Name = "btnCopyLog";
            this.btnCopyLog.Size = new System.Drawing.Size(91, 161);
            this.btnCopyLog.TabIndex = 10;
            this.btnCopyLog.Text = "Copy Log to Clipboard";
            this.btnCopyLog.UseVisualStyleBackColor = true;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(22, 327);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(525, 193);
            this.txtLog.TabIndex = 9;
            this.toolTip1.SetToolTip(this.txtLog, "Click to copy the contents of the log to your clipboard for pasting into an email" +
                    ".");
            // 
            // txtDataUploadToken
            // 
            this.txtDataUploadToken.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataUploadToken.Location = new System.Drawing.Point(186, 163);
            this.txtDataUploadToken.Name = "txtDataUploadToken";
            this.txtDataUploadToken.Size = new System.Drawing.Size(458, 25);
            this.txtDataUploadToken.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtDataUploadToken, "The Data Upload Token you copied from the Online Time Clock MTS Import Data page." +
                    "");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 17);
            this.label3.TabIndex = 28;
            this.label3.Text = "Data Upload Token";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(18, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(161, 21);
            this.label7.TabIndex = 31;
            this.label7.Text = "Export File Location";
            // 
            // txtExportFileLocation
            // 
            this.txtExportFileLocation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExportFileLocation.Location = new System.Drawing.Point(22, 228);
            this.txtExportFileLocation.Name = "txtExportFileLocation";
            this.txtExportFileLocation.Size = new System.Drawing.Size(525, 25);
            this.txtExportFileLocation.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtExportFileLocation, "The location of the export file you\'re going to create");
            // 
            // btnBrowseExportLocation
            // 
            this.btnBrowseExportLocation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseExportLocation.Location = new System.Drawing.Point(553, 228);
            this.btnBrowseExportLocation.Name = "btnBrowseExportLocation";
            this.btnBrowseExportLocation.Size = new System.Drawing.Size(91, 25);
            this.btnBrowseExportLocation.TabIndex = 7;
            this.btnBrowseExportLocation.Text = "Browse";
            this.toolTip1.SetToolTip(this.btnBrowseExportLocation, "Click to browse to a folder to store your export file in.");
            this.btnBrowseExportLocation.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // chkNoFormValidation
            // 
            this.chkNoFormValidation.AutoSize = true;
            this.chkNoFormValidation.Location = new System.Drawing.Point(264, 193);
            this.chkNoFormValidation.Name = "chkNoFormValidation";
            this.chkNoFormValidation.Size = new System.Drawing.Size(115, 17);
            this.chkNoFormValidation.TabIndex = 32;
            this.chkNoFormValidation.Text = "No Form Validation";
            this.chkNoFormValidation.UseVisualStyleBackColor = true;
            // 
            // chkNoValidityChecks
            // 
            this.chkNoValidityChecks.AutoSize = true;
            this.chkNoValidityChecks.Location = new System.Drawing.Point(399, 193);
            this.chkNoValidityChecks.Name = "chkNoValidityChecks";
            this.chkNoValidityChecks.Size = new System.Drawing.Size(115, 17);
            this.chkNoValidityChecks.TabIndex = 33;
            this.chkNoValidityChecks.Text = "No Validity Checks";
            this.chkNoValidityChecks.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 544);
            this.Controls.Add(this.chkNoValidityChecks);
            this.Controls.Add(this.chkNoFormValidation);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtExportFileLocation);
            this.Controls.Add(this.btnBrowseExportLocation);
            this.Controls.Add(this.txtDataUploadToken);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnCreateExportFile);
            this.Controls.Add(this.btnCopyLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.txtAccountAdminLogin);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDatabasePath);
            this.Controls.Add(this.btnBrowse);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Online Time Clock MTS Import File Creator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAccountAdminLogin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAccount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDatabasePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnCreateExportFile;
        private System.Windows.Forms.Button btnCopyLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtDataUploadToken;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtExportFileLocation;
        private System.Windows.Forms.Button btnBrowseExportLocation;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chkNoFormValidation;
        private System.Windows.Forms.CheckBox chkNoValidityChecks;
    }
}

