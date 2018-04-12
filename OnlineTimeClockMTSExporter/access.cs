using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;
using Newtonsoft;

namespace OnlineTimeClockMTSImportFileCreator
{
    public delegate void LogEventHandler(object sender, LogEventArgs e);  

    class access
    {
        string sDatabasePath;
        string sLog = string.Empty;

        OleDbConnection conn = new OleDbConnection();

        public event LogEventHandler LogEvent;

        /// <summary>
        /// constructor
        /// </summary>
        public access(string pDatabasePath)
        {
            this.DatabasePath = pDatabasePath;
        }

        #region json conversion 

        /// <summary>
        /// cycles through each table and gets row data into a JObject and creates a collection of JObjects, one for each valid table
        /// </summary>

        public Newtonsoft.Json.Linq.JObject generateDatabaseTables()
        {
            Newtonsoft.Json.Linq.JObject exportTables;
            Newtonsoft.Json.Linq.JObject exportTable;
            DataTable tables;
            string sTableName = string.Empty;

            this.clearLog();

            try
            {
                tables = this.getTables();

                Int32 iTableNameColumn = tables.Columns.IndexOf("TABLE_NAME");
                exportTables = new Newtonsoft.Json.Linq.JObject();

                foreach (DataRow row in tables.Rows)
                {
                    sTableName = row[iTableNameColumn].ToString();

                    if (sTableName.Substring(0, 3).ToLower() == "tbl")
                    {
                        this.OnLogEvent(new LogEventArgs("generateDatabaseTables Reading Table " + sTableName));
                        exportTable = new Newtonsoft.Json.Linq.JObject();
                        if (sTableName.ToLower() == "tbltimes")
                        {
                            exportTable = this.createTimesExportRows(sTableName, exportTable);
                        }
                        else
                        {
                            exportTable = this.createExportRows(sTableName, exportTable);
                        }
                        if (exportTable.Count > 0)
                        {
                            this.OnLogEvent(new LogEventArgs("generateDatabaseTables Data Created for Table " + sTableName));
                            exportTables.Add(sTableName, exportTable);
                        }
                        else
                        {
                            throw new System.InvalidOperationException("generateDatabaseTables::Failed to retrieve rows for table " + sTableName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.writeLog("generateDatabaseTables :: " + e.ToString());
                exportTables = new Newtonsoft.Json.Linq.JObject();
            }
            this.closeConn();
            return exportTables;

        }


        /// <summary>
        /// selects all rows from a tblTimes, creates a JObject object pair for each time punch pair with the field names and adds the row JObject to a parent table JObject
        /// </summary>
        /// 
        private Newtonsoft.Json.Linq.JObject createTimesExportRows(string pTableName, Newtonsoft.Json.Linq.JObject pTable)
        {
            Int32 iLastEmployeeID=0, iCurrentEmployeeID=0;
            Boolean bCurrentAction=true;
            Newtonsoft.Json.Linq.JObject exportRow;
            Newtonsoft.Json.Linq.JObject exportPunch;
            Newtonsoft.Json.Linq.JObject exportTable = pTable;
            string sTableName = pTableName;
            Int32 iPunchCounter = 0, iRowCounter=0;
            try
            {
                DataTable fields = this.getTableFields(pTableName);
                Int32 iFieldNameColumn = fields.Columns.IndexOf("COLUMN_NAME");
                Int32 iDataTypeColumn = fields.Columns.IndexOf("DATA_TYPE");
                DbDataReader dataReader;
                dataReader = this.returnDataReader("select tblTimes.* from " + sTableName + " inner join tblEmployees on tblTimes.lngEmployeeID=tblEmployees.lngID where tblEmployees.blnDeleted=FALSE order by tblTimes.lngEmployeeID ASC, tblTimes.datEvent ASC");
                if (dataReader.HasRows)
                {
                    exportRow = new Newtonsoft.Json.Linq.JObject();
                    while (dataReader.Read())
                    {

                        bCurrentAction = dataReader.GetBoolean(dataReader.GetOrdinal("blnEventType"));
                        iCurrentEmployeeID=dataReader.GetInt32(dataReader.GetOrdinal("lngEmployeeID"));

                        //new employee add export row
                        if (iCurrentEmployeeID != iLastEmployeeID)
                        {
                            if (iLastEmployeeID > 0)
                            {
                                iPunchCounter++;
                                exportTable.Add(iPunchCounter.ToString(), exportRow);
                                exportRow = new Newtonsoft.Json.Linq.JObject();
                            }
                        }
                        else
                        {
                            if (bCurrentAction) // if an in punch and the exportRow contains some punches then add the punch pair
                            {
                                if (exportRow.Count > 0) //
                                {
                                    iPunchCounter++;
                                    exportTable.Add(iPunchCounter.ToString(), exportRow);
                                    exportRow = new Newtonsoft.Json.Linq.JObject();
                                }
                            }
                        }
                        iLastEmployeeID = iCurrentEmployeeID;
                        exportPunch = new Newtonsoft.Json.Linq.JObject();
                        foreach (DataRow row in fields.Rows)
                        {
                            this.createExportJSONField(exportPunch, dataReader, row, iDataTypeColumn, row[iFieldNameColumn].ToString());
                        }

                        exportRow.Add((bCurrentAction)?"In":"Out", exportPunch);

                        
                        /*
                         * Write log message to show something is still happening
                         */
                        iRowCounter++;
                        if (iRowCounter % 100 == 0)
                        {
                            this.OnLogEvent(new LogEventArgs("createTimesExportRows processing time punch pair  " + iPunchCounter.ToString() + " (" + sTableName + ")", true));
                        }
                    }
                    //add last time punch to collection
                    if (exportRow != null && exportRow.Count > 0)
                    {
                        iPunchCounter++;
                        exportTable.Add(iPunchCounter.ToString(), exportRow);
                    }
                }
                else
                {
                    exportTable.Add("NoRows", 1);
                }
            }
            catch (Exception e)
            {
                this.writeLog("createTimesExportRows :: " + e.ToString());
                exportTable = new Newtonsoft.Json.Linq.JObject();
            }
            return exportTable;
        }

        /// <summary>
        /// selects all rows from a table, creates a JObject for each row with the field names and adds the row JObject to a parent table JObject
        /// </summary>
        /// 
        private Newtonsoft.Json.Linq.JObject createExportRows(string pTableName, Newtonsoft.Json.Linq.JObject pTable)
        {
            Newtonsoft.Json.Linq.JObject exportRow;
            Newtonsoft.Json.Linq.JObject exportTable = pTable;
            string sTableName = pTableName;
            Int32 iRowCounter = 0;
            try
            {
                DataTable fields = this.getTableFields(pTableName);
                Int32 iFieldNameColumn = fields.Columns.IndexOf("COLUMN_NAME");
                Int32 iDataTypeColumn = fields.Columns.IndexOf("DATA_TYPE");
                DbDataReader dataReader;
                dataReader = this.returnDataReader("select * from " + sTableName);
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        exportRow = new Newtonsoft.Json.Linq.JObject();
                        foreach (DataRow row in fields.Rows)
                        {
                           this.createExportJSONField(exportRow,dataReader,row,iDataTypeColumn, row[iFieldNameColumn].ToString());
                        }
                        iRowCounter++;
                        exportTable.Add(iRowCounter.ToString(), exportRow);
                        /*
                         * Write log message to show something is still happening
                         */

                        if (iRowCounter % 100 == 0)
                        {
                            this.OnLogEvent(new LogEventArgs("createExportRows Reading from row " + iRowCounter.ToString() + " (" + sTableName + ")", true));
                        }
                    }
                }
                else
                {
                    exportTable.Add("NoRows", 1);
                }
            }
            catch (Exception e)
            {
                this.writeLog("createExportRows :: " + e.ToString());
                exportTable = new Newtonsoft.Json.Linq.JObject();
            }
            return exportTable;
        }
        /// <summary>
        /// adds a field to a JSON object using a DataRow as a source
        /// </summary>
        /// 

        public void createExportJSONField(Newtonsoft.Json.Linq.JObject exportRow, DbDataReader dataReader, DataRow row, Int32 iDataTypeColumn, string sFieldName)
        {

            if (this.fieldIsBoolean(row[iDataTypeColumn]))
            {
                try
                {

                    if (dataReader.GetBoolean(dataReader.GetOrdinal(sFieldName)))
                    {
                        exportRow.Add(sFieldName, 1);
                    }
                    else
                    {
                        exportRow.Add(sFieldName, 0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsBoolean " + e.ToString());
                }
            }

            if (this.fieldIsDate(row[iDataTypeColumn]))
            {
                try
                {
                    exportRow.Add(sFieldName, dataReader.GetDateTime(dataReader.GetOrdinal(sFieldName)));
                    //exportRow.Add(sFieldName, ((DateTime)dataReader[sFieldName]).ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsDate " + e.ToString());
                }
            }
            if (this.fieldIsInt(row[iDataTypeColumn]))
            {
                try
                {
                    //exportRow.Add(sFieldName, dataReader.GetInt32(iValueColumn).ToString());
                    exportRow.Add(sFieldName, dataReader.GetInt32(dataReader.GetOrdinal(sFieldName)));
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsInt " + e.ToString());

                }
            }
            if (this.fieldIsSingle((Int32)row[iDataTypeColumn]) || this.fieldIsDouble(row[iDataTypeColumn]) || this.fieldIsCurrency(row[iDataTypeColumn]))
            {
                try
                {
                    //exportRow.Add(sFieldName, dataReader.GetDouble(iValueColumn).ToString());
                    exportRow.Add(sFieldName, dataReader.GetValue(dataReader.GetOrdinal(sFieldName)).ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsSingle " + e.ToString());
                }
            }

            if (this.fieldIsString(row[iDataTypeColumn]))
            {
                try
                {
                    //exportRow.Add(sFieldName, dataReader.GetValue(iValueColumn).ToString());
                    exportRow.Add(sFieldName, dataReader[sFieldName].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsString " + e.ToString());
                }
            }
            if (this.fieldIsByte(row[iDataTypeColumn]))
            {
                try
                {
                    //exportRow.Add(sFieldName, dataReader.GetValue(iValueColumn).ToString());
                    exportRow.Add(sFieldName, dataReader.GetValue(dataReader.GetOrdinal(sFieldName)).ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("error converting fieldIsByte " + e.ToString());
                }
            }

        }

#endregion
        #region Database Validity Checks

        /// <summary>
        /// runs through each of the database validity checks
        /// </summary>

        public bool checkValidDatabase(application application)
        {
            bool bReturn = true;

            try
            {
                //this.OnLogEvent(new LogEventArgs("Checking that Time Clock MTS Database Can be Opened"));
                this.openConn();
            }
            catch (Exception e)
            {
                if (e.GetType().Name == typeof(System.Data.OleDb.OleDbException).Name)
                {
                    this.OnLogEvent(new LogEventArgs("Cannot open " + this.DatabasePath + ".  The file might be corrupt or not even a valid database."));
                }
                else
                {
                    this.OnLogEvent(new LogEventArgs("An unhandled exception has occurred trying to open " + this.DatabasePath));
                    this.OnLogEvent(new LogEventArgs(e.ToString()));
                }
                bReturn = false;

            }


            if (bReturn)
            {
                this.OnLogEvent(new LogEventArgs("Checking that Primary Keys Exist"));
                bReturn = checkPrimaryKeysExist(application);
            }

            if (bReturn)
            {
                this.OnLogEvent(new LogEventArgs("Primary Keys Exist!")); 
                this.OnLogEvent(new LogEventArgs("Checking for duplicate time ID's"));  
                bReturn = checkNoDuplicateTimeID(application);
            }

            if (bReturn)
            {
                this.OnLogEvent(new LogEventArgs("No duplicate time ID's!")); 
                this.OnLogEvent(new LogEventArgs("Checking first time punches for each employee"));  
                bReturn = checkFirstPunchType(application);
            }

            if (bReturn)
            {
                this.OnLogEvent(new LogEventArgs("First time punches valid for each employee!")); 
                this.OnLogEvent(new LogEventArgs("Checking that time punch structure is valid"));  
                bReturn = checkValidTimeStructure(application);
            }

            if (bReturn)
            {
                this.OnLogEvent(new LogEventArgs("The time punch structure is valid!"));  
            }
            this.closeConn();
            return bReturn;
        }
        /// <summary>
        /// checks each employee's time records to make sure they are in the correct in/out sequence
        /// </summary>
        private bool checkValidTimeStructure(application application)
        {
            bool bReturn = true, bFirstRowForEmployees = false, bLastEvent = false; 
            Int32 iLastEmployeeID = 0;
            List<string> sBadStructureMessages=new List<string>();

            string sSql="select tblTimes.lngID, tblTimes.blnEventType, tblTimes.lngEmployeeID, tblEmployees.strFullName from tblTimes inner join tblEmployees on tblTimes.lngEmployeeID=tblEmployees.lngID where tblEmployees.blnDeleted=FALSE order by tblEmployees.lngID ASC, tblTimes.datEvent DESC";
            DbDataReader dataReader;

            try
            {
                dataReader=this.returnDataReader(sSql);
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (iLastEmployeeID!=(Int32) dataReader["lngEmployeeID"])
                        {
                            this.OnLogEvent(new LogEventArgs("Checking time structure for employee "+dataReader["strFullName"].ToString()+"("+dataReader["lngEmployeeID"].ToString()+")"));  
                            bFirstRowForEmployees=true;
                        }
                        else
                        {
                            bFirstRowForEmployees=false;
                        }
                        if (!bFirstRowForEmployees)
                        {
                            if (bLastEvent == (bool)dataReader["blnEventType"])
                            {
                                bReturn = false;
                                sBadStructureMessages.Add("Bad time structure at TimeID "+dataReader["lngID"].ToString()+" for employee "+dataReader["strFullName"].ToString()+"("+dataReader["lngEmployeeID"].ToString()+")");
                            }
                        }
                        bLastEvent = (bool)dataReader["blnEventType"];
                        iLastEmployeeID = (Int32)dataReader["lngEmployeeID"];
                        bFirstRowForEmployees = false;
                    }
                }
                //write all bad structure messages to end of log
                string[] sMessage = sBadStructureMessages.ToArray();
                if (sMessage.Length > 0)
                {
                    for (Int32 i = 0; i < sMessage.Length; i++)
                    {
                        this.OnLogEvent(new LogEventArgs(sMessage[i])); 
                    }
                }
            }
            catch(Exception e)
            {
                bReturn = false;
                this.writeLog("access.checkValidTimeStructure " + e.ToString());
            }
            return bReturn;
        }
        /// <summary>
        /// checks that the first punch for each employee is an IN
        /// </summary>
        private bool checkFirstPunchType(application application)
        {
            bool bReturn = true;
            string sSqlEmployees = "select lngID, strFullName from tblEmployees where blnDeleted=FALSE order by strFullName ASC;";
            string sSQLTimePunch = string.Empty;
            DbDataReader dataReaderEmployees, dataReaderTimePunch;
            OleDbConnection connLocal;
            connLocal = this.createDatabaseConnection();
            try
            {
                dataReaderEmployees = this.returnDataReader(sSqlEmployees,null,connLocal);
                if (dataReaderEmployees.HasRows)
                {
                    while (dataReaderEmployees.Read())
                    {
                        Console.WriteLine("Loading times for "+dataReaderEmployees["strFullName"].ToString()+ "("+dataReaderEmployees["lngID"]+")");
                        dataReaderTimePunch = this.returnDataReader("select top 1 datEvent, blnEventType, lngID from tblTimes where lngEmployeeID=" + (Int32)dataReaderEmployees["lngID"]+" order by datEvent ASC", null, connLocal);
                        if (dataReaderTimePunch.HasRows)
                        {
                            while (dataReaderTimePunch.Read())
                            {
                                if ((bool)dataReaderTimePunch["blnEventType"] == false)
                                {
                                    bReturn = false;
                                    this.OnLogEvent(new LogEventArgs("Bad first time punch for " + dataReaderEmployees["strFullName"].ToString() + "(Employee ID:" + dataReaderEmployees["lngID"].ToString() + " Time ID: " + dataReaderTimePunch["lngID"].ToString() + ")"));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                bReturn = false;
                this.writeLog("access.checkFirstPunchType " + e.ToString());
            }
            return bReturn;
        }
        /// <summary>
        /// checks that there are no duplicate time ID's in the time table.  This can happen if the database gets corrupted.
        /// </summary>
        private bool checkNoDuplicateTimeID(application application)
        {
            bool bReturn = true;
            DataTable tables;
            string sTableName = string.Empty;
            string sFieldName = string.Empty;

            this.clearLog();
            tables = this.getTables();
            this.clearLog();

            try
            {

                DbDataReader dataReader;
                dataReader = this.returnDataReader("select count(tblTimes.lngID) as count_id, lngID from tblTimes group by tblTimes.lngID having count(tblTimes.lngID)>1;");
                if (dataReader.HasRows)
                {
                    this.OnLogEvent(new LogEventArgs("access.checkNoDuplicateTimeID duplicate primary keys found in tblTimes"));
                    bReturn = false;
                }
                dataReader.Close();
            }
            catch (Exception e)
            {
                bReturn = false;
                this.writeLog("access.checkNoDuplicateTimeID " + e.ToString());
            }
            return bReturn;

        }
        /// <summary>
        /// checks that all the tables have a primary key.  Ignores system tables.
        /// </summary>
        private bool checkPrimaryKeysExist(application application)
        {
            bool bReturn = true; 
            DataTable tables;
            string sTableName = string.Empty;
            string sFieldName = string.Empty;

            this.clearLog();
            tables = this.getTables();
            this.clearLog();

            Int32 iTableNameColumn = tables.Columns.IndexOf("TABLE_NAME");

            foreach (DataRow row in tables.Rows)
            {
                sTableName = row[iTableNameColumn].ToString();

                if (sTableName.Substring(0, 3).ToLower() == "tbl")
                {
                    DataTable fields = this.getTableFields(sTableName);

                    Int32 iFieldNameColumn = fields.Columns.IndexOf("COLUMN_NAME");
                    Int32 iDataTypeColumn = fields.Columns.IndexOf("DATA_TYPE");
                    Int32 iMaxLengthColumn = fields.Columns.IndexOf("CHARACTER_MAXIMUM_LENGTH");

                    this.writeLog("access.checkValidDatabase :: checking for PrimaryKey in " + sTableName);

                    if (this.getAccessPrimaryKeyName(sTableName).Length == 0)
                    {
                        bReturn = false;
                        this.writeLog("access.checkValidDatabase:: Table " + sTableName + " has no primary key");
                        break;
                    }
                }
            }

            return bReturn;

        }

        #endregion
        #region database access
        /// <summary>
        /// returns a data reader with SQL and can parameterize date time query parameters
        /// </summary>
        public DbDataReader returnDataReader(string sSQL, DateTime[] datParameterValues = null, OleDbConnection pConn = null, bool pSequentialAccess=false)
        {
            DbCommand command;
            DbDataReader dataReader;
            CommandBehavior behavior = CommandBehavior.Default;

            if (pSequentialAccess)
            {
                behavior = CommandBehavior.SequentialAccess;
            }

            OleDbConnection connLocal;

            if (pConn == null)
            {
              connLocal = this.createDatabaseConnection();
            }
            else
                connLocal = pConn;

            //this.openConn();
            
            command = this.returnCommand(sSQL, datParameterValues, connLocal);
            
            try
            {

                dataReader = command.ExecuteReader(behavior);
                return dataReader;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                do
                {

                } while (connLocal.State == ConnectionState.Connecting);
                dataReader = command.ExecuteReader(behavior);
                return dataReader;
            }
        }
        /// <summary>
        /// gets the primary key of a given table
        /// </summary>
        public string getAccessPrimaryKeyName(string pTableName)
        {
            var returnList = new List<string>();

            OleDbConnection conOleDb = new OleDbConnection();
            this.openConn();
            conOleDb = (OleDbConnection)conn;

            DataTable mySchema = conOleDb.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys,
                                        new Object[] { null, null, pTableName });
            conOleDb.Dispose();

            // following is a lengthy form of the number '3' :-)
            int columnOrdinalForName = mySchema.Columns["COLUMN_NAME"].Ordinal;

            if (mySchema.Rows.Count > 0)
                return mySchema.Rows[0].ItemArray[columnOrdinalForName].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// executes a non query
        /// </summary>
        private int executeNonQuery(string pSQL, DateTime[] datParameterValues = null)
        {


            OleDbCommand command;
            Int32 iReturn = 0;

            using (OleDbConnection _conn = this.createDatabaseConnection())
            {
                try
                {
                    command = returnCommand(pSQL, datParameterValues = null, _conn);

                    iReturn = command.ExecuteNonQuery();
                    command.Dispose();
                }
                catch (Exception e)
                {
                    this.writeLog("executeNonQuery :: " + e.ToString());
                }
            }


            return iReturn;

        }
        /// <summary>
        /// returns an oledbcommand object with the given sql and can parameterize date time parameters
        /// </summary>
        private OleDbCommand returnCommand(string sSQL, DateTime[] datParameterValues = null, OleDbConnection connLocal = null)
        {
            OleDbCommand command;

            command = new OleDbCommand();

            if (connLocal == null)
                this.openConn();

            try
            {

                if (connLocal == null)
                    command.Connection = conn;
                else
                    command.Connection = connLocal;

                command.CommandText = sSQL;
                //
                //Setup parameters if required
                //
                if (datParameterValues != null)
                {
                    OleDbParameter parm;

                    parm = new OleDbParameter();
                    for (int i = 0; i < datParameterValues.Length; i++)
                    {
                        parm = command.CreateParameter();
                        parm.Value = datParameterValues[i];
                        parm.DbType = DbType.DateTime;

                        command.Parameters.Add(parm);
                    }
                }
            }
            catch (Exception e)
            {
                //this.closeConn();
                this.writeLog("returnCommand :: " + e.ToString());
            }

            //this.closeConn();
            return command;
        }
        /// <summary>
        /// returns a list of the database tables in a DataTable object
        /// </summary>
        public DataTable getTables()
        {
            DataTable table = new DataTable();
            List<string> lstFields = new List<string>();
            this.writeLog("Getting existing tables");
            try
            {
                this.openConn();
                OleDbConnection conSQL = new OleDbConnection();
                conSQL = conn;
                table = conSQL.GetSchema("Tables");
                conSQL.Dispose();
            }
            catch (Exception e)
            {
                this.closeConn();
                this.writeLog("getTables :: " + e.ToString());
            }
            //System.Data.DataRow row = table.Rows[0];
            this.closeConn();
            return table;

        }
        /// <summary>
        /// returns the columns for a given database table in a DataTable object
        /// </summary>
        public DataTable getTableFields(string pTable)
        {
            DataTable table = new DataTable();
            List<string> lstFields = new List<string>();
            string[] restrictions = new string[4];

            try
            {
                OleDbConnection conSQL = new OleDbConnection();
                this.openConn();
                conSQL = (OleDbConnection)conn;
                restrictions[2] = pTable;
                table = conSQL.GetSchema("Columns", restrictions);
                conSQL.Dispose();
            }
            catch (Exception e)
            {
                this.closeConn();
                this.writeLog("getTableFields :: " + e.ToString());
            }
            //System.Data.DataRow row = table.Rows[0];
            this.closeConn();
            return table;
        }
        /// <summary>
        /// opens an oledb connection object
        /// </summary>
        private void openConn()
        {
            if (conn.State == ConnectionState.Closed)
            {
                if (conn.ConnectionString.Length == 0)
                    this.createDatabaseConnection();
                else
                    conn.Open();
            }
        }
        /// <summary>
        /// closes an oledb connection object
        /// </summary>
        private void closeConn()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
                //conn.Dispose();
            }
        }

        /// <summary>
        /// creates an oledb connection object
        /// </summary>
        private OleDbConnection createDatabaseConnection()
        {
            Int32 iRetries = 0;

            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.DataSource = this.DatabasePath;
            builder.Add("Provider", "Microsoft.Jet.OLEDB.4.0");
            builder.Add("Mode", "16");
            //builder.Add("Mode", "3");
            builder.Add("Jet OLEDB:Database Locking Mode", "1");
            //sPassword=this.sAccessPassword;
            //if (sPassword.Length>0)
            //     builder.Add("Database Password",sPassword);
            //Console.WriteLine(builder.ConnectionString.ToString());
            OleDbConnection oleDBConnection = new OleDbConnection(builder.ConnectionString);

            try
            {
                oleDBConnection.Open();
            }
            catch (Exception e)
            {

                while (iRetries < 40 && oleDBConnection.State != ConnectionState.Open)
                {
                    iRetries++;
                    Random rand = new Random();

                    System.Threading.Thread.Sleep(rand.Next(25, 50));

                    if (oleDBConnection.State != ConnectionState.Open && oleDBConnection.State != ConnectionState.Connecting)
                        oleDBConnection.Open();

                }

                if (iRetries >= 40 || oleDBConnection.State != ConnectionState.Open)
                {
                    this.writeLog("access.createDatabaseConnection Retries: " + iRetries + ":: ConnectionState:" + oleDBConnection.State.ToString());
                    this.writeLog("access.createDatabaseConnection " + e.ToString());
                }
            }

            conn = oleDBConnection;

            return oleDBConnection;

        }

        #endregion
        #region database data type checking
        /// <summary>
        /// checks if a passed data type from an access table is boolean
        /// </summary>
        public Boolean fieldIsBoolean(object oDataType)
        {
            Boolean bBoolean = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.Boolean) bBoolean = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsBoolean " + e.ToString());
            }


            return bBoolean;
        }
        /// <summary>
        /// checks if a passed data type from an access table is a string
        /// </summary>
        public Boolean fieldIsString(object oDataType)
        {
            Boolean bString = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.VarWChar || iDataType == (Int32)System.Data.OleDb.OleDbType.LongVarWChar || iDataType == (Int32)System.Data.OleDb.OleDbType.WChar) bString = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsString " + e.ToString());
            }

            return bString;
        }
        /// <summary>
        /// checks if a passed data type from an access table is a date
        /// </summary>
        public Boolean fieldIsDate(object oDataType)
        {
            Boolean bDate = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.Date) bDate = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsDate " + e.ToString());
            }
            return bDate;
        }
        /// <summary>
        /// checks if a passed data type from an access table is an int
        /// </summary>
        public Boolean fieldIsInt(object oDataType)
        {
            Boolean bInt = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.SmallInt || iDataType == (Int32)System.Data.OleDb.OleDbType.Integer || iDataType == (Int32)System.Data.OleDb.OleDbType.BigInt) bInt = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsInt " + e.ToString());
            }
            return bInt;
        }
        /// <summary>
        /// checks if a passed data type from an access table is a byte
        /// </summary>
        public Boolean fieldIsByte(object oDataType)
        {
            Boolean bByte = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.UnsignedTinyInt) bByte = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsByte " + e.ToString());
            }
            return bByte;
        }
        /// <summary>
        /// checks if a passed data type from an access table is a single
        /// </summary>
        public Boolean fieldIsSingle(object oDataType)
        {
            Boolean bSingle = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.Single) bSingle = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsSingle " + e.ToString());
            }
            return bSingle;
        }
        /// <summary>
        /// checks if a passed data type from an access table is a double
        /// </summary>
        public Boolean fieldIsDouble(object oDataType)
        {
            Boolean bSingle = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.Double) bSingle = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsDouble " + e.ToString());
            }
            return bSingle;
        }
        /// <summary>
        /// checks if a passed data type from an access table is access currency type
        /// </summary>
        public Boolean fieldIsCurrency(object oDataType)
        {
            Boolean bSingle = false;
            Int32 iDataType;

            try
            {
                iDataType = System.Convert.ToInt32(oDataType);
                if (iDataType == (Int32)System.Data.OleDb.OleDbType.Currency) bSingle = true;
            }
            catch (Exception e)
            {
                this.writeLog("access.fieldIsCurrency " + e.ToString());
            }
            return bSingle;
        }

        #endregion
        #region data logging
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

        public string DatabasePath
        {
            get { return this.sDatabasePath; }
            set { this.sDatabasePath = value.Trim(); }
        }

        protected void OnLogEvent(LogEventArgs e)
        {
            if (LogEvent != null)
                LogEvent(this, e);
        }

    #endregion

    }
}
