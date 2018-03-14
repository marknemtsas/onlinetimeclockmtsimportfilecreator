using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Nini;
using OnlineTimeClockMTSImportFileCreator.Properties;

namespace OnlineTimeClockMTSImportFileCreator
{
    /// <summary>
    /// class to store application wide settings and methods
    /// </summary>
    public class application
    {
        string _sErrorMessage = string.Empty;

        /// <summary>
        /// Looks for and reads the [Program Settings]->DatabaseLoc ini file value from the Time Clock MTS INI File
        /// </summary>
        private string sReadIniValue()
        {
            string sValue = "";
            string sSection = "Program Settings";
            string sKey = "DatabaseLoc";


            //
            //Load up ini file
            //
            if (!bFileExists(this.sCommonAppDataDirectory + "\\timeclock.ini"))
            {
                this.ErrorMessage = this.sCommonAppDataDirectory + "\\timeclock.ini does not exist";
                return sValue;
            }

            try
            {
                Nini.Config.IniConfigSource source = new Nini.Config.IniConfigSource(this.sCommonAppDataDirectory + "\\timeclock.ini");
                //
                //Check if section exists, if not return
                //
                if (!bIniSectionExists(source, sSection))
                {
                    this.ErrorMessage = "[" + sSection + "] does not exist";
                    return sValue;
                }
                //
                //Check if key exists, create if it doesn't and assign default value
                //
                if (!source.Configs[sSection].Contains(sKey))
                {
                    this.ErrorMessage = "[" + sKey + "] does not exist";
                    return sValue;
                }
                else
                {

                    //
                    //Read value
                    //
                    sValue = source.Configs[sSection].GetString(sKey);

                }
                return sValue;
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.ToString();
                return sValue;
            }
        }

        public string ErrorMessage
        {
            get { return _sErrorMessage; }
            set { _sErrorMessage = value; }

        }
        /// <summary>
        /// Checks if a section exists in an INI file
        /// </summary>
        private bool bIniSectionExists(Nini.Config.IniConfigSource source, string sSection)
        {
            bool bReturn = false;

            foreach (Nini.Config.IniConfig config in source.Configs)
            {
                if (config.Name == sSection)
                    return true;
            }
            return bReturn;
        }
        /// <summary>
        /// Gets the CommonAppData diectory
        /// </summary>
        public string sCommonAppDataDirectory
        {
            get
            {
                //string s = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + '\\' + Application.ProductName;
                string s = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Time Clock MTS";
                return s;
            }
        }
        /// <summary>
        /// Checks if a directory exists
        /// </summary>
        public bool bDirectoryExists(string sPath = "")
        {
            return System.IO.Directory.Exists(sPath);
        }
        /// <summary>
        /// Checks if a file exists
        /// </summary>
        public bool bFileExists(string sPath = "")
        {
            return System.IO.File.Exists(sPath);
        }

        public string ExportPath
        {
            get
            {
                this.ErrorMessage = string.Empty;
                string sReturn = string.Empty;
                if (Settings.Default.ExportPath != null)
                {
                    sReturn = Settings.Default.ExportPath;
                }
                return sReturn;
            }
        }
        public string AccountName
        {
            get
            {
                this.ErrorMessage = string.Empty;
                string sReturn = string.Empty;
                if (Settings.Default.AccountName != null)
                {
                    sReturn = Settings.Default.AccountName;
                }
                return sReturn;
            }
        }
        public string AccountLogin
        {
            get
            {
                this.ErrorMessage = string.Empty;
                string sReturn = string.Empty;
                if (Settings.Default.AccountLogin != null)
                {
                    sReturn = Settings.Default.AccountLogin;
                }
                return sReturn;
            }
        }

        public string DatabasePath
        {
            get
            {
                this.ErrorMessage = string.Empty;
                string sReturn = string.Empty;
                if (Settings.Default.DatabasePath != null)
                {
                    sReturn = Settings.Default.DatabasePath;
                }
                if (sReturn.Length == 0)
                {
                    sReturn = this.sReadIniValue();
                }
                return sReturn;
            }

        }
        /// <summary>
        /// Saves the application settings
        /// </summary>
        public void saveSettings(string pExportPath,
            string pAccountName,
            string pAccountLogin,
            string pDatabasePath)
        {
            Settings.Default.ExportPath = pExportPath.Trim();
            Settings.Default.AccountName = pAccountName.Trim();
            Settings.Default.AccountLogin = pAccountLogin.Trim();
            Settings.Default.DatabasePath = pDatabasePath.Trim();
            Settings.Default.Save();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

    }



}
