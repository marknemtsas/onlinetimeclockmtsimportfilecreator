using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;

namespace OnlineTimeClockMTSImportFileCreator
{
    class exportFile
    {
        public event LogEventHandler LogEvent;
        string sLog = string.Empty;
        /// <summary>
        /// constructor
        /// </summary>
        public exportFile(access pAccess, string pSavePath, string pAccount, string pAccountAdminLogin, string pAccountAdminPassword, string pDataUploadToken)
        {
            this.exportHeader = new exportHeader( pAccount,  pAccountAdminLogin,  pAccountAdminPassword,  pDataUploadToken);
            this.savePath = pSavePath;
            this.access = pAccess;
        }
        /// <summary>
        /// calls each step in the process of creating the export file
        /// </summary>
        public bool createExportFile()
        {
            bool bReturn = true;

            bReturn=this.createExportObject();

            if (bReturn)
            {
                bReturn = this.saveExportObject();
            }

            return bReturn;
        }
        /// <summary>
        /// creates the the JObject containing the header and the data for each table
        /// </summary>
        private bool createExportObject()
        {
            bool bReturn = true;

            try
            {
                this.exportObject = new Newtonsoft.Json.Linq.JObject();
                this.exportObject.Add("Header", new Newtonsoft.Json.Linq.JObject(Newtonsoft.Json.Linq.JObject.FromObject(this.exportHeader)));
                Newtonsoft.Json.Linq.JObject tables = this.access.generateDatabaseTables();
                if (tables.Count > 0)
                {
                    this.exportObject.Add("Data", tables);
                }
                else
                {
                    throw new System.InvalidOperationException("createExportObject::Failed to get tables object");
                }
            }
            catch(Exception e)
            {
                this.OnLogEvent(new LogEventArgs("createExportObject " + e.ToString())); 
                bReturn = false;
            }
            return bReturn;
        }
        /// <summary>
        /// Saves the JObject to a file.
        /// </summary>
        private bool saveExportObject()
        {
            bool bReturn = true;

            try
            {
                System.IO.File.WriteAllText(this.savePath, Newtonsoft.Json.JsonConvert.SerializeObject(this.exportObject, Newtonsoft.Json.Formatting.Indented));
            }
            catch (Exception e)
            {
                this.OnLogEvent(new LogEventArgs("saveExportObject " + e.ToString()));
                bReturn = false;
            }
            return bReturn;


        }
        #region properties
        private Newtonsoft.Json.Linq.JObject exportObject { get; set; }
        private exportHeader exportHeader { get; set; }
        private string savePath { get; set; }
        private access access { get; set; }
        #endregion
        #region logging
        private void clearLog()
        {
            this.sLog = string.Empty;
        }

        private void writeLog(string pMessage)
        {
            OnLogEvent(new LogEventArgs(pMessage));
        }

        public string Log
        {
            get { return this.sLog; }
        }


        protected void OnLogEvent(LogEventArgs e)
        {
            if (LogEvent != null)
                LogEvent(this, e);
        }
        #endregion

    }
    /// <summary>
    /// helper class to hold information for the export file header
    /// </summary>
    internal class exportHeader
    {
        public exportHeader(string pAccount, string pAccountAdminLogin, string pAccountAdminPassword, string pDataUploadToken)
        {
            this.Account = pAccount;
            this.AccountAdminLogin = pAccountAdminLogin;
            this.AccountAdminPassword = pAccountAdminPassword;
            this.DataUploadToken = pDataUploadToken;
        }

        public string Account { get; set; }
        public string AccountAdminLogin { get; set;}
        public string AccountAdminPassword { get; set;}
        public string DataUploadToken { get; set; }
    }



    
}
