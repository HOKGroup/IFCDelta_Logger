using Autodesk.Revit.UI;
using Revit.IFC.Common.Utility;
using RevitLogger.UI;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevitLogger.SQLite_Helper
{
    class SQLiteUtil
    {
        static SQLiteUtil _instance;
        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader dr;
        List<string> SQLcommandList = new List<string>();
        public string _fullpath = "";
        public void CreateDatabaseAndTables(string FullPath)
        {

            // this will be replaced with the logger path

            string loggerDatbaseName = "LoggerDB.sqlite";



            _fullpath = (FullPath.LastOrDefault() != '\\' ? FullPath += @"\" : FullPath) + loggerDatbaseName;
            if (!File.Exists(_fullpath))
            {
                SQLiteConnection.CreateFile(_fullpath);

                string sql = @"CREATE TABLE Project(
                               ProjectID                INTEGER     NOT NULL PRIMARY KEY   UNIQUE,
                               ProjectName              TEXT        NOT NULL UNIQUE,
                               ProjectNumber            TEXT        NOT NULL,
                               Lat                      REAL,
                               Lon                      REAL,
                               ProjectCreationDate      TEXT        NOT NULL,
                               ExternalUniqueID         TEXT        UNIQUE
                            );  


                            CREATE TABLE Files( 
                               FileGUID                 TEXT        NOT NULL PRIMARY KEY  UNIQUE,
                               FileName                 TEXT        NOT NULL,
                               FileFormat               TEXT        NOT NULL,
                               FileCreationDate         TEXT        NOT NULL,
                               ProjectID                INTEGER             ,
                               FOREIGN KEY(ProjectID) REFERENCES Project(ProjectID)

                            );  




                            CREATE TABLE Elements(  
                               ElementGUID              TEXT        NOT NULL PRIMARY KEY  UNIQUE,
                               ElementAppID             TEXT                ,
                               FileGUID                 TEXT                ,
                               ElementType              TEXT        NOT NULL,
                               ElementFirstCaptureDate  TEXT                ,
                               ElementDeactivatedDate   TEXT                ,
                               ElementViewSpecific      INTEGER             ,
                               ElementHostViewID        TEXT                ,
                               ElementIsType              INTEGER             ,
                               IFCGuid                  TEXT        NOT NULL,

                               FOREIGN KEY(FileGUID) REFERENCES Files(FileGUID)

                            );




                            CREATE TABLE Transactions(  
                               FileVersionGUID          TEXT        NOT NULL PRIMARY KEY  UNIQUE,
                               FileGUID                 TEXT        NOT NULL,
                               TransactionCreationDate  TEXT        NOT NULL,
                               TransactionUser          TEXT                ,

                               FOREIGN KEY(FileGUID) REFERENCES Files(FileGUID)

                            );



                            CREATE TABLE ElementTransactions(  
                               ElementTrasactionID      INTEGER     NOT NULL PRIMARY KEY  UNIQUE  ,
                               ElementVersionGUID       TEXT        NOT NULL,
                               ElementGUID              TEXT                ,
                               FileVersionGUID          TEXT                ,
                               ElementName              TEXT                ,
                               ElementTransactionAction TEXT                ,
                               ProjectNote              TEXT                ,
                               UserNote                 TEXT                ,
                               UserNoteUserName         TEXT                ,
                               ElementTransactionDate   TEXT                ,
                               
                               
                               FOREIGN KEY(FileVersionGUID) REFERENCES Transactions(FileVersionGUID),
                               FOREIGN KEY(ElementGUID) REFERENCES Elements(ElementGUID)

                            );

                            CREATE TABLE ElementTransactionGeom(  
                               ElementTransGeomID       INTEGER     NOT NULL PRIMARY KEY  UNIQUE  ,
                               FileVersionGUID          TEXT                ,
                               ElementGUID              TEXT                ,
                               BoundingBoxMin           BLOB                ,
                               BoundingBoxMax           BLOB                ,
                               LEVEL                    BLOB                ,
                               Rotation                 FLOAT               ,
                               ElemTransGeomDate        TEXT                ,
                               
                               FOREIGN KEY(FileVersionGUID) REFERENCES Transactions(FileVersionGUID),
                               FOREIGN KEY(ElementGUID) REFERENCES Elements(ElementGUID)

                            );

                            CREATE TABLE ElementTransactionValues(  
                               ElemTransactionValuesID      INTEGER     NOT NULL PRIMARY KEY  UNIQUE  ,
                               FileVersionGUID              TEXT                ,
                               ElementGUID                  TEXT                ,
                               ElemTransactionPropName      TEXT                ,
                               ElemTransactionPropValue     TEXT                ,
                               ElemTransactionPropDataType  TEXT                ,
                               ElemTransactionPropDataDate  TEXT                ,
                               
                               FOREIGN KEY(FileVersionGUID) REFERENCES Transactions(FileVersionGUID),
                               FOREIGN KEY(ElementGUID) REFERENCES Elements(ElementGUID)

                            );
                  ";

                try
                {
                    CreateDBConnection(_fullpath);

                    con.Open();
                    cmd = new SQLiteCommand(sql, con);
                    cmd.ExecuteNonQuery();
                    CloseDBConnection();

                }
                catch (Exception ex)
                {
                    CloseDBConnection();
                    throw;
                }

            }
            else
            {
                string sql = @"  select count(*) from
                                pragma_table_info('Elements')
                                where name = 'IFCGuid'; ";



                CreateDBConnection(_fullpath);
                con.Open();
                cmd = new SQLiteCommand(sql, con);
                var dr = cmd.ExecuteReader();
                int temp = -2;
                while (dr.Read())
                {

                    temp = Convert.ToInt32(dr[0]);


                }

                if (temp == 0)

                {
                    sql = "ALTER TABLE Elements ADD COLUMN IFCGuid TEXT";
                    cmd = new SQLiteCommand(sql, con);
                    cmd.ExecuteNonQuery();
                }

                CloseDBConnection();
            }
        }


        public void CreateDBConnection(string path)
        {

            CloseDBConnection();
            con = new SQLiteConnection($"Data Source={path};Version=3;");
        }

        public void CloseDBConnection()
        {

            if (con?.ConnectionString != null)
            {
                con.Close();
                //con = null;

            }

        }




        public void AddProjectTableData(string ProjectName, string ProjectNumber, float Lat, float Lon, DateTime ProjectCreationDate, string ExternalUniqueID)
        {
            var ProjectCreationDateStr = ProjectCreationDate.ToString("yyyyMMdd HHmmss");
            SQLcommandList.Add($"insert into Project(ProjectName,ProjectNumber,Lat,Lon,ProjectCreationDate,ExternalUniqueID) " +
                $"values ('{ProjectName}','{ProjectNumber}',{Lat},{Lon},'{ProjectCreationDateStr}','{ExternalUniqueID}');");
        }
        public void AddFilesTableData(string FileGUID, string FileName, string FileFormat, DateTime FileCreationDate, int ProjectID)
        {
            var FileCreationDateStr = FileCreationDate.ToString("yyyyMMdd HHmmss");
            SQLcommandList.Add($"insert into " +
                $"Files(FileGUID,FileName,FileFormat,FileCreationDate,ProjectID) " +
                $"values ('{FileGUID}','{FileName}','{FileFormat}','{FileCreationDateStr}',{ProjectID});");
        }

        public void AddElementsTableData(string ElementGUID, string ElementAppID, string FileGUID, string ElementType, DateTime ElementFirstCaptureDate, int ElementViewSpecific, string ElementHostViewID, int ElementIsType, string IFCGuid)
        {
            // in add command we can't detect element delete date so it will be not inserted to be null
            var ElementFirstCaptureDateStr = ElementFirstCaptureDate.ToString("yyyyMMdd HHmmss");
            SQLcommandList.Add($"insert into " +
                $"Elements(ElementGUID,ElementAppID,FileGUID,ElementType,ElementFirstCaptureDate,ElementViewSpecific,ElementHostViewID,ElementIsType,IFCGuid) " +
                $"values ('{ElementGUID}','{ElementAppID}','{FileGUID}','{ElementType}','{ElementFirstCaptureDateStr}','{ElementViewSpecific}','{ElementHostViewID}',{ElementIsType},'{IFCGuid}');");
        }

        public void AddTransactionsTable(string FileVersionGUID, string FileGUID, DateTime TransactionCreationDate, string TransactionUser)
        {
            var notFound = QueryDataBase(Tables.Transactions, $"FileVersionGUID ='{FileVersionGUID}'").Count == 0;
            
            if (notFound)
            {
                var TransactionCreationDateStr = TransactionCreationDate.ToString("yyyyMMdd HHmmss");
                SQLcommandList.Add($"insert into " +
                    $"Transactions(FileVersionGUID,FileGUID,TransactionCreationDate,TransactionUser) " +
                    $"values ('{FileVersionGUID}','{FileGUID}','{TransactionCreationDateStr}','{TransactionUser}');");

            }
        }

        public void AddElementTransactionsTable(string ElementVersionGUID, string ElementGUID, string FileVersionGUID, string ElementName, string ElementTransactionAction, string ProjectNote, string UserNote, string UserNoteUserName, DateTime ElementTransactionDate)
        {
            var ElementTransactionDateStr = ElementTransactionDate.ToString("yyyyMMdd HHmmss");
            var ElementNameStr = ElementName;
            if (ElementName.Contains("'"))
            {
                ElementNameStr = ElementName.Replace("'", "''");
            }
            SQLcommandList.Add($"insert into " +
                $"ElementTransactions(ElementVersionGUID,ElementGUID,FileVersionGUID,ElementName,ElementTransactionAction,ProjectNote,UserNote,UserNoteUserName,ElementTransactionDate) " +
                $"values ('{ElementVersionGUID}','{ElementGUID}','{FileVersionGUID}','{ElementNameStr}','{ElementTransactionAction}','{ProjectNote}','{UserNote}','{UserNoteUserName}','{ElementTransactionDateStr}');");
        }

        public void AddElementTransactionGeomTable(string FileVersionGUID, string ElementGUID, string BoundingBoxMin, string BoundingBoxMax, string LEVEL, float Rotation, DateTime ElemTransGeomDate)
        {

            var ElemTransGeomDateStr = ElemTransGeomDate.ToString("yyyyMMdd HHmmss");

            SQLcommandList.Add($"insert into " +
                $"ElementTransactionGeom(FileVersionGUID,ElementGUID,BoundingBoxMin,BoundingBoxMax,LEVEL,Rotation,ElemTransGeomDate) " +
                $"values ('{FileVersionGUID}','{ElementGUID}','{BoundingBoxMin}','{BoundingBoxMax}','{LEVEL}',{Rotation},'{ElemTransGeomDateStr}');");
        }

        //we will update ElementTable on delete 
        public void UpdateElementsTableDataByElementGUID(string ElementGUID, DateTime ElementDeactivatedDate)
        {
            var ElementDeactivatedDateStr = ElementDeactivatedDate.ToString("yyyyMMdd HHmmss");
            SQLcommandList.Add($"UPDATE " +
                $"Elements SET ElementDeactivatedDate = '{ElementDeactivatedDateStr}'" +
                $"WHERE  ElementGUID ='{ElementGUID}';");
        }
        public void UpdateElementsTableDataByAppElementId(string ElementId, DateTime ElementDeactivatedDate)
        {
            var ElementDeactivatedDateStr = ElementDeactivatedDate.ToString("yyyyMMdd HHmmss");
            SQLcommandList.Add($"UPDATE " +
                $"Elements SET ElementDeactivatedDate = '{ElementDeactivatedDateStr}'" +
                $"WHERE  ElementAppID ='{ElementId}';");
        }

        public void UpdateFileTableDataByFileGUID(string FileGUID, int ProjectID)
        {
            SQLcommandList.Add($"UPDATE " +
                        $"Files SET ProjectID = '{ProjectID}'" +
                        $"WHERE  FileGUID ='{FileGUID}';");

        }


        public void Execute(bool UpdateProgressBar, Prog progrss1)
        {

            cmd = new SQLiteCommand();
            con.Open();
            cmd.Connection = con;
            string errorMessage = "";
            var portion = 0d;
            if (UpdateProgressBar)
            {

                portion = (double)(((100 - Helpers.Globals.progressBarValue)) / (SQLcommandList.Count + 1));


            }
           RevitStatusBar revitStatusBar = RevitStatusBar.Create();
            int counter = 1;
            foreach (var commandText in SQLcommandList)
            {
                try
                {
                    Helpers.Globals.progressBarValue += portion;
                    revitStatusBar.Set($"Excute Command to DB {counter} of {SQLcommandList.Count}. ");
                    progrss1.UpdateProgressBarValue();
                    cmd.CommandText = commandText.Replace("'NULL'", "NULL");
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    errorMessage += ex.Message + "\n";
                    continue;
                }
                counter++;

            }
            SQLcommandList.Clear();
            con.Close();

            if (errorMessage != "")
            {
                MessageBox.Show("These Exceptions had been raised adding the data to DB.\n" + errorMessage);
            }
            Helpers.Globals.progressBarValue = 101;

            progrss1.UpdateProgressBarValue();

        }

        public List<List<object>> QueryDataBase(Tables TableToQuery, string Condition = null)
        {




      var WhereCondtions = string.IsNullOrEmpty(Condition) ? ";" : "WHERE " + Condition + ";";

            var command = $"SELECT * FROM {TableToQuery} {WhereCondtions}";








            List<List<object>> AllResult = new List<List<object>>();
            cmd = new SQLiteCommand(command, con);
            con.Open();


            try
            {
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    List<object> oneResult = new List<object>();

                    switch (TableToQuery)
                    {
                        case Tables.Project:

                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);
                            oneResult.Add(dr[5]);
                            oneResult.Add(dr[6]);



                            break;
                        case Tables.Files:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);



                            break;
                        case Tables.Elements:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);
                            oneResult.Add(dr[5]);
                            oneResult.Add(dr[6]);
                            oneResult.Add(dr[7]);
                            oneResult.Add(dr[8]);
                            oneResult.Add(dr[9]);

                            break;
                        case Tables.Transactions:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            break;
                        case Tables.ElementTransactions:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);
                            oneResult.Add(dr[5]);
                            oneResult.Add(dr[6]);
                            oneResult.Add(dr[7]);
                            oneResult.Add(dr[8]);
                            oneResult.Add(dr[9]);


                            break;
                        case Tables.ElementTransactionGeom:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);
                            oneResult.Add(dr[5]);
                            oneResult.Add(dr[6]);
                            oneResult.Add(dr[7]);
                            oneResult.Add(dr[8]);
                            oneResult.Add(dr[9]);
                            break;
                        case Tables.ElementTransactionValues:
                            oneResult.Add(dr[0]);
                            oneResult.Add(dr[1]);
                            oneResult.Add(dr[2]);
                            oneResult.Add(dr[3]);
                            oneResult.Add(dr[4]);
                            oneResult.Add(dr[5]);
                            oneResult.Add(dr[6]);

                            break;
                        default:
                            break;
                    }

                    AllResult.Add(oneResult);
                }






            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", "SQLLiteUtil File Query DB\n" + ex.Message);

            }

            con.Close();
            return AllResult;
        }


        private SQLiteUtil()
        {

        }
        public static SQLiteUtil CreateSQLite()
        {
            if (_instance == null)
            {
                SQLiteUtil instance = new SQLiteUtil();
                _instance = instance;
                return instance;
            }

            else
            {
                return _instance;
            }

        }

        public void Dispose()
        {
            try
            {
                if (dr != null)
                {
                    dr.Close();

                    con.Dispose();
                    cmd.Dispose();
                    SQLiteUtil._instance = null;
                }

            }
            catch (Exception ex)
            {

                TaskDialog.Show("Error", "SqlLiteUtil File DisposeMethod\n" + ex.Message);


            }
        }


    }

    public enum Tables
    {
        Project,
        Files,
        Elements,
        Transactions,
        ElementTransactions,
        ElementTransactionGeom,
        ElementTransactionValues,
    }
}
