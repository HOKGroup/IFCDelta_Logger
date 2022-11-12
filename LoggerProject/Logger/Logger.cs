using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIM.IFC.Export.UI;
using Helpers;
using MoreLinq;
using Revit.IFC.Export.Exporter;
using Revit.IFC.Export.Utility;
using RevitLogger.Helpers;
using RevitLogger.SQLite_Helper;
using RevitLogger.UI;
using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Reflection;
using System.Windows;
using _settings = RevitLogger.Settings.Settings;

namespace RevitLogger
{
    internal class Logger : ExternalEventInfo
    {
        public static string modelGuid { get; set; }
        public static int RevitVersion { get; set; }
        public IFCHeaderData _IFCHeaderData;

        Prog progrss1;
        static Document _doc;
        public static UIApplication uIApplication { get; set; }
        bool _firstSave;

        public Logger(Prog progressWindow, Document doc, bool firstSave)
        {
            progrss1 = progressWindow;
            _doc = doc;
            _firstSave = firstSave;
            _IFCHeaderData = new IFCHeaderData();

        }
        public void Log()
        {

            //to determine which scope we will take (All - Model Categories - Annotation Categories )
            //string scope = Settings.Settings.scope;


            FilteredElementCollector FamilyInstanceCollector = new FilteredElementCollector(_doc);

            //get all elements in the model that is not element type 
            IList<Element> ElementsCollection = FamilyInstanceCollector.WhereElementIsNotElementType().ToElements();

            //lists to obtain the elements according to the scope 
            IList<Element> AllModelElements = new List<Element>();
            IList<Element> AllAnnotationElements = new List<Element>();
            IList<Element> AllCustomElements = new List<Element>();
            IList<Element> AllOtherCatElements = new List<Element>();
            List<Element> scopedElements = new List<Element>();



            foreach (Element e in ElementsCollection)
            {
                if (e.Category == null) continue;




                // get element geometry to check it's model or not
                var geom = e.get_Geometry(new Options());


                //Get Model Elements in the Revit document
                //
                //check if the element has geometry and its category is model if the scope is (model/all/custom) which surly not annotation  
                if (null != geom && e.Category.CategoryType == CategoryType.Model && Settings.Settings.isModelCatFilter)
                {
                    foreach (var item in geom)
                    {

                        var soild = item as Solid;

                        if (soild != null)
                        {
                            if (soild.Volume > 0 && e.Category.CategoryType == CategoryType.Model && e.Location != null)
                            {

                                AllModelElements.Add(e);


                            }
                        }

                        else if (item.IsElementGeometry)
                        {


                            AllModelElements.Add(e);


                        }
                    }
                }




                // get annotation elements in the model
                //check the ategory type and the scope either.
                if (e.Category.CategoryType == CategoryType.Annotation && e.Location != null && Settings.Settings.isAnnotationCatFilter)
                {
                    AllAnnotationElements.Add(e);
                    continue;
                }


                if (_settings.isViewFilter && e is View)
                {
                    var eView = e as View;
                    if (_settings.isScheduleFilter && eView.ViewType == ViewType.Schedule)
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }


                    if (_settings.isDetailFilter && eView.ViewType == ViewType.Detail)
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }

                    if (_settings.isDetailFilter && eView.ViewType == ViewType.Detail)
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }
                    if (_settings.isSheetFilter && eView.ViewType == ViewType.DrawingSheet)
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }
                    if (_settings.isModelViewsFilter && (eView.ViewType == ViewType.FloorPlan || eView.ViewType == ViewType.CeilingPlan || eView.ViewType == ViewType.EngineeringPlan))
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }
                    if (_settings.isLegendsFilter && eView.ViewType == ViewType.Legend)
                    {
                        AllCustomElements.Add(e);
                        continue;
                    }




                }

                if (_settings.isFamilyTypesFilter && e is FamilySymbol)
                {
                    AllCustomElements.Add(e);
                    continue;
                }
                if (_settings.isLinksFilter && e is RevitLinkInstance)
                {
                    AllCustomElements.Add(e);
                    continue;
                }
                if (_settings.isImportedFilter && e is ImportInstance)
                {
                    AllCustomElements.Add(e);
                    continue;
                }

                var scopedCount = _settings.scope?.Where(x => e.Category.Name.ToLower().Contains(x.ToLower())).Count();
                if (scopedCount > 0)
                {
                    AllOtherCatElements.Add(e);
                }


            }




            // get the desired elements upone the selected scope by user to be porceed to next steps 

            scopedElements.AddRange(AllModelElements);
            scopedElements.AddRange(AllAnnotationElements);
            scopedElements.AddRange(AllCustomElements);
            scopedElements.AddRange(AllOtherCatElements);

            scopedElements = scopedElements.DistinctBy(p => p.UniqueId).ToList();


            SQLiteUtil sQLiteUtil = SQLiteUtil.CreateSQLite();

            Exporter.IfcHeaderData = null;





            //var dupicated = scopedElements.GroupBy(x => x.UniqueId).Where(g => g.Count() > 1).ToList();
            sQLiteUtil.CreateDatabaseAndTables(_settings.FullLogPath);

            var ProjectListzz = sQLiteUtil.QueryDataBase(Tables.Project);
            var ProjectList = sQLiteUtil.QueryDataBase(Tables.Project, $"ProjectName = '{Settings.Settings.ProjectName}'");
            //Generate Unique File Guid 
            ElementId projectLevelElementId = new ElementId(-16 /*project*/);
            var FileGUID = ExportUtils.GetExportId(_doc, projectLevelElementId).ToString();
            var date = DateTime.Now;
            Exporter.date = date;
            if (ProjectList.Count == 0)
            {
                //If the ProjectList is empyty That means that this is a new project so we will add the project data then we will add file data to be 
                //assigned to this project. 


                Guid id = Guid.NewGuid();
                String sID = id.ToString();
                sQLiteUtil.AddProjectTableData(_settings.ProjectName, _settings.ProjectNumber, 0f, 0f, date, _settings.externalProjectID);
                sQLiteUtil.Execute(false, progrss1);




            }

            var fileIsFound = CheckFileLogIsFoundAndUpdate(sQLiteUtil, FileGUID, date);

            _IFCHeaderData.ModelID = FileGUID;

            Exporter.ExportIFCperElement = !_settings.DeltaFileExport;


            if (Logger.RevitVersion < 2023)
            {

                if (fileIsFound)
                {
                    GetElementFromDBToCompareWithExistOnes(scopedElements, FileGUID, date);
                }
                else
                {
                    string FileVersionGUID = CreateFileVersionGuidForTransaction();
                    _IFCHeaderData.ModelVersionId = FileVersionGUID;
                    AddDocumentElementToElementTable(scopedElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, true);
                }
            }
            else if (_doc.IsWorkshared)
            {
                if (fileIsFound)
                {
                    GetModifiedDeletedNewElementFromDocumentDiff(_doc, FileGUID, date);
                }
                else
                {
                    string FileVersionGUID = CreateFileVersionGuidForTransaction();
                    _IFCHeaderData.ModelVersionId = FileVersionGUID;
                    AddDocumentElementToElementTable(scopedElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, true);
                }





            }
            else
            {

                if (fileIsFound)
                {
                    //this is because documentDiffrence doesn't track deleted elements in non-shared working files
                    string FileVersionGUID = CreateFileVersionGuidForTransaction();
                    _IFCHeaderData.ModelVersionId = FileVersionGUID;
                    sQLiteUtil.AddTransactionsTable(FileVersionGUID, FileGUID, date, _doc.Application.Username);
                    GetModifiedNewElementFromDocumentDiff(scopedElements, _doc, FileGUID, date, FileVersionGUID);
                    GetDeletedElementFromDB(scopedElements, date, FileVersionGUID, FileGUID);
                }
                else
                {
                    string FileVersionGUID = CreateFileVersionGuidForTransaction();
                    _IFCHeaderData.ModelVersionId = FileVersionGUID;
                    AddDocumentElementToElementTable(scopedElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, true);
                }

            }












            sQLiteUtil.Execute(true, progrss1);

            sQLiteUtil.CloseDBConnection();


            var test2 = Assembly.GetAssembly(typeof(Logger)).Location;
            var directory = Path.GetDirectoryName(test2);
            //Assembly.LoadFrom("")

            var assm = Assembly.GetAssembly(typeof(Exporter));
            var test = assm.Location;


            Directory.CreateDirectory(_settings.FullLogPath + @"\IFC\Settings");

            //IFC Export By Element
            if (_settings.IfcOnOff)
            {
                Exporter.IfcHeaderData = _IFCHeaderData;

                Exporter.logPath = _settings.FullLogPath;
                IFCCommandOverrideApplication iFCCommand = new IFCCommandOverrideApplication();
                iFCCommand.logPath = _settings.FullLogPath;
                iFCCommand.OnIFCExport(uIApplication, null);

            }

            //string loggerFileText = "";

            ////we will give a 70% of progresss bar to logg element
            ////and 20% comparing modifing elements
            ////and 10% to new elements 
            //var portion = (double)(50d / (scopedElements.Count + 1));

            //Helpers.Globals.progressBarValue = 0;

            ////var ss = _settings.LogPath;
            ////var ss2 = _settings.FullLogPath;


            ////wwe wil loop to each element in our scoped element 
            //foreach (var element in scopedElements)
            //{
            //    Helpers.Globals.progressBarValue += portion;
            //    progrss1.UpdateProgressBarValue();

            //    logElement logElement = new logElement(element);

            //    // get the logger text as jsonl format
            //    var loggerOfElement = logElement.GetLoogerText(LoggerType.log);


            //    //we check if the logger text is not empty on the empty bounding box. this step is double check not to log any wrong element
            //    if (loggerOfElement == null)
            //        continue;

            //    // finally we add this json line to the rest of lines to be exported as file in next steps 
            //    loggerFileText += $"{loggerOfElement}\n";



            //}







            ////generate UTC time format 
            //var UTCdate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            ////make sure if the directory of logger path + (projectName_projectNumber) is found, if not we will create it
            //bool exist = Directory.Exists(Settings.Settings.FullLogPath);
            //if (!exist)
            //    Directory.CreateDirectory(Settings.Settings.FullLogPath);


            ////get external project id
            //string ExternalProjectId = Settings.Settings.externalProjectID;
            //if (string.IsNullOrEmpty(ExternalProjectId))
            //    ExternalProjectId = "00001";

            //try
            //{

            //    // if we save the project for the first time in save as command so we dont have any changes in change file
            //    if (_firstSave == true)
            //    {

            //        File.WriteAllText(Settings.Settings.FullLogPath + $"{_doc.Title}_{modelGuid}_{ExternalProjectId}_{UTCdate}_change.jsonl", "");

            //    }

            //    //if we save it regurally so we have to check the files before to make cahnge file
            //    else
            //    {
            //        // we get the change file inner text by comparing the previous files..  if there is no previous files the change file will be empty though.
            //        var changeFileText = CompareLogFiles(scopedElements, _doc);

            //        //writing the file with the changes
            //        File.WriteAllText(Settings.Settings.FullLogPath + $"{_doc.Title}_{modelGuid}_{ExternalProjectId}_{UTCdate}_change.jsonl", changeFileText);

            //    }

            //    //finally we write the log file it self .. we print it last to not be get involved in the comparing last log files process as it would considered the last log files
            //    //so to avoid this by writing extra checking code we print it at last step.
            //    File.WriteAllText(Settings.Settings.FullLogPath + $"{_doc.Title}_{modelGuid}_{ExternalProjectId}_{UTCdate}_log.jsonl", loggerFileText);
            //    Helpers.Globals.progressBarValue = 101;
            //    progrss1.UpdateProgressBarValue();
            //}
            //catch (Exception ex)
            //{

            //    TaskDialog.Show("error", ex.Message);
            //}




        }

        private void GetDeletedElementFromDB(List<Element> scopedElements, DateTime date, string FileVersionGUID, string FileGUID)
        {
            var sqLite = SQLiteUtil.CreateSQLite();
            //1-Get File Elements from DB 
            var allDBElements = sqLite.QueryDataBase(Tables.Elements, $"FileGUID = '{FileGUID}'");
            var allDBElementsUniqueIds = allDBElements.Select(x => x[0]).Cast<string>().ToList();
            var scopedElementsUniaueIds = scopedElements.Select(x => x.UniqueId).ToList();
            var DeletedElementsFromDB = allDBElementsUniqueIds.Except(scopedElementsUniaueIds).ToList();
            DeletedElementsFromDB.AddRange(scopedElementsUniaueIds.Except(allDBElementsUniqueIds).ToList());
            List<string> DeletedElements = new List<string>();

            foreach (var eleUniqueId in DeletedElementsFromDB)
            {

                Element element = _doc.GetElement(eleUniqueId);
                var flag = sqLite.QueryDataBase(Tables.Elements, $"ElementGUID ='{eleUniqueId}' AND ElementDeactivatedDate IS NULL");
                if (element == null && flag.Count > 0)
                {
                    DeletedElements.Add(eleUniqueId);
                }

            }

            foreach (var deletedElem in DeletedElements)
            {

                sqLite.UpdateElementsTableDataByElementGUID(deletedElem.ToString(), date);
                AddDeletedElementsToTransactionTableByEleGUID(deletedElem, FileVersionGUID, date);

            }


        }

        private void GetModifiedNewElementFromDocumentDiff(List<Element> scopedElements, Document doc, string FileGUID, DateTime date, string FileVersionGUID)
        {
            var sqlite = SQLiteUtil.CreateSQLite();

            var lastTransactionRecord = sqlite.QueryDataBase(Tables.Transactions);
            if (lastTransactionRecord != null && lastTransactionRecord?.Count > 0)
                try
                {
                    _IFCHeaderData.PreviousModelVersionID = lastTransactionRecord.Last()[0].ToString();
                    Guid lastTransaction = new Guid(lastTransactionRecord.Last()[0].ToString());
                    //hope this works for transaction Id or we will generate a guid every transaction to over come this problem.
                    //DocVersion = new Guid("fbf83587-39ba-4f11-8a9a-c511c2618436");
                    DocumentDifference diff = doc.GetChangedElements(lastTransaction);
                    var created = diff.GetCreatedElementIds();

                    var modified = diff.GetModifiedElementIds();
                    var deleted = diff.GetDeletedElementIds();
                    List<Element> newElements = new List<Element>();
                    List<Element> modifiedElements = new List<Element>();






                    foreach (var item in created)
                    {
                        var ele = _doc.GetElement(item);
                        if (scopedElements.Select(x => x.Id).Contains(ele.Id))
                            newElements.Add(ele);

                    }

                    foreach (var item in modified)
                    {

                        var ele = _doc.GetElement(item);
                        if (scopedElements.Select(x => x.Id).Contains(ele.Id))
                            modifiedElements.Add(ele);

                    }

                    if (newElements.Count > 0)
                        AddDocumentElementToElementTable(newElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, false);


                    foreach (var element in modifiedElements)
                    {

                        AddElementsToElementTransactionAndElementTransGeom(FileGUID, _doc.Application.Username, FileVersionGUID, element, ElementTransactionAction.MODIFIED, date);
                    }

                    Exporter.allExportedEle.AddRange(newElements);
                    Exporter.allExportedEle.AddRange(modifiedElements);






                }
                catch (Exception ex)
                {
                    MessageBox.Show("Some Error Has happened:\n" + ex.Message);
                }
        }

        private void GetModifiedDeletedNewElementFromDocumentDiff(Document doc, string FileGUID, DateTime date)
        {

            var sqlite = SQLiteUtil.CreateSQLite();

 

            var lastTransactionRecord = sqlite.QueryDataBase(Tables.Transactions);
            if (lastTransactionRecord != null)
                try
                {
                    Guid lastTransaction = new Guid(lastTransactionRecord.Last()[0].ToString());
                    _IFCHeaderData.PreviousModelVersionID = lastTransactionRecord.Last()[0].ToString();
                    var FileVersionGUID = CreateFileVersionGuidForTransaction();
                    if (lastTransaction.ToString() != FileVersionGUID)
                    {
                        sqlite.AddTransactionsTable(FileVersionGUID, FileGUID, date, _doc.Application.Username);

                    }


                    DocumentDifference diff = doc.GetChangedElements(lastTransaction);
                    var created = diff.GetCreatedElementIds();

                    var modified = diff.GetModifiedElementIds();
                    var deleted = diff.GetDeletedElementIds();
                    List<Element> newElements = new List<Element>();
                    List<Element> modifiedElements = new List<Element>();



                    foreach (var item in created)
                    {
                        newElements.Add(_doc.GetElement(item));

                    }

                    foreach (var item in modified)
                    {
                        modifiedElements.Add(_doc.GetElement(item));
                    }

                    Exporter.allExportedEle.AddRange(newElements);
                    Exporter.allExportedEle.AddRange(modifiedElements);

                    if (newElements.Count > 0)
                        AddDocumentElementToElementTable(newElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, false);

                    if (modifiedElements.Count > 0)
                        foreach (var element in modifiedElements)
                        {

                            AddElementsToElementTransactionAndElementTransGeom(FileGUID, _doc.Application.Username, FileVersionGUID, element, ElementTransactionAction.MODIFIED, date);
                        }



                    foreach (var deletedElem in deleted)
                    {
                        var result = AddDeletedElementsToTransactionTableByEleID(deletedElem.ToString(), FileVersionGUID, date);
                        if (!result)
                            continue;
                        sqlite.UpdateElementsTableDataByAppElementId(deletedElem.ToString(), date);

                    }





                }
                catch (Exception ex)
                {

                    MessageBox.Show("Some Error Has happened:\n" + ex.Message);

                }
        }

        private void AddDocumentElementToElementTable(List<Element> scopedElements, string FileGUID, string UserName, string FileVersionGUID, DateTime date, bool makeTransaction)
        {

            //Common Fields among steps
            var _userName = string.IsNullOrEmpty(UserName) ? "-" : UserName;







            //1-first We will add All scoped elements to Element table
            var sqLiteUtil = SQLiteUtil.CreateSQLite();
            foreach (var element in scopedElements)
            {
                var IsElementViewSpecific = element.ViewSpecific ? 1 : 0;
                var HostID = element.ViewSpecific ? element.OwnerViewId.ToString() : "NULL";
                var IsElementType = element.GetType().IsSubclassOf(typeof(ElementType)) ? 1 : 0;


                var IFCGuid = element.get_Parameter(BuiltInParameter.IFC_GUID).AsValueString();

                sqLiteUtil.AddElementsTableData(element.UniqueId, element.Id.ToString(), FileGUID, element.Category.Name, date, IsElementViewSpecific, HostID, IsElementType, IFCGuid);


                //Step 3 -- we do it here to save an extra loop 
                //Step 4 -- we also do it here to save an extra looping step
                AddElementsToElementTransactionAndElementTransGeom(FileGUID, _userName, FileVersionGUID, element, ElementTransactionAction.NEW, date);

            }


            // 2- Add Transaction Data To Transaction Table 
            // we check for maketransaction value as if we add we may add a transaction before this step if
            // the file is already found in DB so a transaction for Modified elements has been happen and 
            // perform another entry into Db with the same  FileVersionGUID will cause errors
            // as it's unique value per each transaction
            if (makeTransaction)
                sqLiteUtil.AddTransactionsTable(FileVersionGUID, FileGUID, date, _userName);


            // 3 - Add TransactionElements to Table 
            // we already added them in the first step to save extra loop 

            // 4 -  Add TransactionElementsGeom to Table 
            // Also we already added them in the first step to save extra loop (⊙…⊙ ) <(¬＿¬<)

            //5- Add  ElementTransactionValues --> We be added later
            //sqLiteUtil.Excute();

            Exporter.allExportedEle.AddRange(scopedElements);

        }

        private static string CreateFileVersionGuidForTransaction()
        {

            if (Logger.RevitVersion < 2023)
            {


                Guid id = Guid.NewGuid();  // Unique GUID For Transaction .. Every Transaction has its own unique guid
                String FileVersionGUID = id.ToString();
                
                return FileVersionGUID;

            }
            else if (_doc.IsWorkshared && (_doc.IsModelInCloud))
            {
                string fileName = _doc.WorksharingCentralGUID + ".rvt";
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string collaborationDir = Path.Combine(localAppData, "Autodesk\\Revit\\Autodesk Revit " + _doc.Application.VersionNumber, "CollaborationCache");
                string file = Directory.GetFiles(collaborationDir, fileName, SearchOption.AllDirectories).FirstOrDefault(x => new FileInfo(x).Directory?.Name != "CentralCache");

                BasicFileInfo info = BasicFileInfo.Extract(file);
                DocumentVersion v = info.GetDocumentVersion();
                return v.VersionGUID.ToString();

            }
            else
            {
                string path = _doc.PathName;
                BasicFileInfo info = BasicFileInfo.Extract(path);
                DocumentVersion v = info.GetDocumentVersion();
                return v.VersionGUID.ToString();
            }



        }



        private void AddElementsToElementTransactionAndElementTransGeom(string FileGUID, string _userName, string FileVersionGUID, Element element, ElementTransactionAction elementTransactionAction, DateTime date)
        {

            SQLiteUtil sqLiteUtil = SQLiteUtil.CreateSQLite();
            sqLiteUtil.AddElementTransactionsTable(element.VersionGuid.ToString(), element.UniqueId, FileGUID, element.Name, elementTransactionAction.ToString(), _settings.ProjectNote, _settings.UserNote, _userName, date);

            var bbx = checkElementBoundinBox(element, _doc);
            var bbxMin = bbx == null ? "NULL" : bbx.Min.ToString();
            var bbxMax = bbx == null ? "NULL" : bbx.Max.ToString();
            var Level = element.LevelId == null ? "NULL" : element.LevelId.ToString();
            sqLiteUtil.AddElementTransactionGeomTable(FileVersionGUID, element.UniqueId, bbxMin, bbxMax, Level, 0, date);

        }

        private void AddDeletedElementsToTransactionTableByEleGUID(string DeletedElementUniqueId, string FileVersionGUID, DateTime date)
        {
            SQLiteUtil sqLite = SQLiteUtil.CreateSQLite();
            var LastTransactionDeleted = sqLite.QueryDataBase(Tables.ElementTransactions, $"ElementGUID = '{DeletedElementUniqueId}'  ORDER BY  ElementTransactionDate").LastOrDefault();
            sqLite.AddElementTransactionsTable(LastTransactionDeleted[1].ToString(), LastTransactionDeleted[2].ToString(), FileVersionGUID, LastTransactionDeleted[4].ToString(), ElementTransactionAction.DELETED.ToString(), _settings.ProjectNote, _settings.UserNote, _doc.Application.Username, date);
            var ElementData = sqLite.QueryDataBase(Tables.Elements, $"ElementGUID = '{DeletedElementUniqueId}'").FirstOrDefault();
            ElementData.Add(LastTransactionDeleted[1]);

            if (_settings.IfcOnOff)
            {
                EditDeletedFileFromPreviousLogs(ElementData, date);


            }

        }
        private bool AddDeletedElementsToTransactionTableByEleID(string DeletedElementID, string FileVersionGUID, DateTime date)
        {
            SQLiteUtil sqLite = SQLiteUtil.CreateSQLite();
            var ElementData = sqLite.QueryDataBase(Tables.Elements, $"ElementAppID = '{DeletedElementID}'").FirstOrDefault();
            if (ElementData == null)
                return false;
            var DeltedElementGuid = ElementData[0];
            if (DeltedElementGuid == null)
                return false;
            var LastTransactionDeleted = sqLite.QueryDataBase(Tables.ElementTransactions, $"ElementGUID = '{DeltedElementGuid}'  ORDER BY  ElementTransactionDate").LastOrDefault();
            sqLite.AddElementTransactionsTable(LastTransactionDeleted[1].ToString(), LastTransactionDeleted[2].ToString(), FileVersionGUID, LastTransactionDeleted[4].ToString(), ElementTransactionAction.DELETED.ToString(), _settings.ProjectNote, _settings.UserNote, _doc.Application.Username, date);
            ElementData.Add(LastTransactionDeleted[1]);
            if (_settings.IfcOnOff)
            {
                EditDeletedFileFromPreviousLogs(ElementData, date);

            }

            return true;

        }

        private void EditDeletedFileFromPreviousLogs(List<object> ElementData, DateTime date)
        {

            var ifcGUID = ElementData[9];
            var elementGUID = ElementData[10];
            if (_settings.DeltaFileExport)
            {
                _IFCHeaderData.DeletedItems.Add(ifcGUID.ToString());
                return;
            }

            if (Directory.Exists(_settings.FullLogPath + $@"\IFC\{_doc.Title}\"))
            {

                var AllFiles = Directory.GetFiles(_settings.FullLogPath + $@"\IFC\{_doc.Title}\", "*", SearchOption.AllDirectories);
                var DeletedFile = AllFiles.Where(x => x.Contains(ifcGUID?.ToString()) && x.Contains(elementGUID.ToString())).OrderBy(x => x).LastOrDefault();

                if (!string.IsNullOrEmpty(DeletedFile))
                {
                    var FileName = Path.GetFileName(DeletedFile);

                    var firstSegments = FileName.Substring(0, FileName.Length - 4);
                    var newDeletedFileName = firstSegments + "_Deleted.ifc";
                    var newDeletedPath = _settings.FullLogPath + $@"\IFC\{_doc.Title}\{date.ToString("yyyyMMddHHmmss")}\" + newDeletedFileName;
                    Directory.CreateDirectory(_settings.FullLogPath + $@"\IFC\{_doc.Title}\{date.ToString("yyyyMMddHHmmss")}\");

                    File.Copy(DeletedFile, newDeletedPath, true);

                }
            }
        }

        private BoundingBoxXYZ checkElementBoundinBox(Element element, Document document)
        {
            BoundingBoxXYZ elementBbx = element.get_BoundingBox(document.ActiveView);

            if (elementBbx == null)
            {
                View3D Collector3D = new FilteredElementCollector(element.Document).OfClass(typeof(View3D)).WhereElementIsNotElementType().Cast<View3D>().Where(x => x.IsTemplate == false).FirstOrDefault();
                View collectorView = new FilteredElementCollector(element.Document).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().Cast<ViewPlan>().Where(x => x.IsTemplate == false).FirstOrDefault();
                if (Collector3D == null)
                {
                    var Doc = element.Document;
                    var direction = new XYZ(-1, 1, -1);
                    var collector = new FilteredElementCollector(Doc);
                    var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                      .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);



                    using (Transaction ttNew = new Transaction(Doc, "abc"))
                    {
                        ttNew.Start();
                        var view3D = View3D.CreateIsometric(
                                          Doc, viewFamilyType.Id);

                        view3D.SetOrientation(new ViewOrientation3D(
                          direction, new XYZ(0, 1, 1), new XYZ(0, 1, -1)));


                        ttNew.Commit();
                        elementBbx = element.get_BoundingBox(view3D);

                    }


                }
                else
                {
                    try
                    {
                        elementBbx = element.get_BoundingBox(element.Document.ActiveView);
                        if (elementBbx == null)
                            elementBbx = element.get_BoundingBox(Collector3D as View);

                    }
                    catch (Exception e)
                    {

                        TaskDialog.Show("error", e.Message);
                    }
                }
            }
            return elementBbx;
        }

        private void GetElementFromDBToCompareWithExistOnes(List<Element> scopedElements, string FileGUID, DateTime date)
        {

            string FileVersionGUID = CreateFileVersionGuidForTransaction();
            _IFCHeaderData.ModelVersionId = FileVersionGUID;

            var sqLite = SQLiteUtil.CreateSQLite();
            var lastTransaction = sqLite.QueryDataBase(Tables.Transactions);
            if (lastTransaction != null)
            {

                _IFCHeaderData.PreviousModelVersionID = lastTransaction.Last()[0].ToString();


            }



          //1-Get File Elements from DB 
          var allDBElements = sqLite.QueryDataBase(Tables.Elements, $"FileGUID = '{FileGUID}'");
            var allDBElementsUniqueIds = allDBElements.Select(x => x[0]).Cast<string>().ToList();
            var scopedElementsUniaueIds = scopedElements.Select(x => x.UniqueId).ToList();
            sqLite.AddTransactionsTable(FileVersionGUID, FileGUID, date, _doc.Application.Username);

            //just check if there are any elements on DB Otherwise all scoped elements will be new and we are done her
            if (allDBElements.Count == 0)
            {

                AddDocumentElementToElementTable(scopedElements, FileGUID, _doc.Application.Username, FileVersionGUID, date, false);
                return;

            }


            //2- Now we will classify our elements to  3 Types (New, Modified, Deleted)  based on the elements we get from DB
            //  2-1- Get Modified And Deleted Elements

            List<List<object>> ScopedElementsFromDB = new List<List<object>>();

            var portion = (double)(30d) / (scopedElements.Count + 1);
            foreach (var item in scopedElements)
            {

                Helpers.Globals.progressBarValue += portion;

                progrss1.UpdateProgressBarValue();
                var result = sqLite.QueryDataBase(Tables.Elements, $"ElementGUID = '{item.UniqueId}'").FirstOrDefault();
                if (result != null)
                    ScopedElementsFromDB.Add(result);

            }

            //List<List<object>> ScopedElementsFromDB = allDBElements.Where(x => scopedElements.Any(y => y.UniqueId == x[0].ToString())).ToList();


            var DeletedElementsFromDB = allDBElementsUniqueIds.Except(scopedElementsUniaueIds).ToList();
            DeletedElementsFromDB.AddRange(scopedElementsUniaueIds.Except(allDBElementsUniqueIds).ToList());
            //List<List<object>> RestOfElements = allDBElements.Where(x => scopedElements.All(y => y.UniqueId != x[0].ToString()) && (x[5].GetType().GetProperties().Count() == 0)).ToList();

            List<Element> NewElemetsList = new List<Element>();


            List<Element> ModifiedElements = new List<Element>();
            List<string> DeletedElements = new List<string>();

            foreach (var item in ScopedElementsFromDB)
            {
                Helpers.Globals.progressBarValue += portion;
                progrss1.UpdateProgressBarValue();
                var eleUniqueId = item[0].ToString();
                Element element = _doc.GetElement(eleUniqueId);
                if (element == null)
                {
                    DeletedElements.Add(eleUniqueId);
                }
                else
                {
                    var LastTransactionOnScopedElement = sqLite.QueryDataBase(Tables.ElementTransactions, $"ElementGUID = '{eleUniqueId}'  ORDER BY  ElementTransactionDate").LastOrDefault();
                    var DBElementVersionGUID = LastTransactionOnScopedElement[1].ToString();
                    if (DBElementVersionGUID != element.VersionGuid.ToString())
                    {
                        ModifiedElements.Add(element);
                    }
                }

            }


            foreach (var eleUniqueId in DeletedElementsFromDB)
            {

                Element element = _doc.GetElement(eleUniqueId);
                var flag = sqLite.QueryDataBase(Tables.Elements, $"ElementGUID ='{eleUniqueId}' AND ElementDeactivatedDate IS NULL");
                if (element == null && flag.Count > 0)
                {
                    DeletedElements.Add(eleUniqueId);
                }
                else if (element != null && flag.Count == 0)
                {
                    NewElemetsList.Add(element);
                }
            }


            //for new Items we Will Add them to database
            if (NewElemetsList.Count > 0)
                AddDocumentElementToElementTable(NewElemetsList, FileGUID, _doc.Application.Username, FileVersionGUID, date, false);


            //for Modified Elements will will Add them ElementTransactions into Table
            //and ElementTransactionGeom and ElementTransactionValues(for later) 


            foreach (var element in ModifiedElements)
            {
                AddElementsToElementTransactionAndElementTransGeom(FileGUID, _doc.Application.Username, FileVersionGUID, element, ElementTransactionAction.MODIFIED, date);
            }


            //for Deleted Elements we will update Element Table to set ElementDeactivatedDate  and add them into Transaction Table

            foreach (var deletedElem in DeletedElements)
            {
                sqLite.UpdateElementsTableDataByElementGUID(deletedElem.ToString(), date);
                AddDeletedElementsToTransactionTableByEleGUID(deletedElem, FileVersionGUID, date);
            }


          





        }

        private bool CheckFileLogIsFoundAndUpdate(SQLiteUtil sQLiteUtil, string FileGUID, DateTime date)
        {

            // if the ProjectList has value so we would check if the file is already assigned to this project or not 
            // if the file not found we will create a record of it >> if it found we will update the project Id field

            var FileList = sQLiteUtil.QueryDataBase(Tables.Files, $"FileGUID = '{FileGUID}'");
            var project1 = sQLiteUtil.QueryDataBase(Tables.Project, $"ProjectName = '{Settings.Settings.ProjectName}'").First();

            //Get ProjectId 
            var ProjectId = Convert.ToInt32(project1[0]);
            if (FileList.Count == 0)
            {
                var documentName = Path.GetFileNameWithoutExtension(_doc.Title);
                var docExten = _doc.PathName.Split('.').Last();
                sQLiteUtil.AddFilesTableData(FileGUID, documentName, docExten, date, ProjectId);
            }
            else
            {
                sQLiteUtil.UpdateFileTableDataByFileGUID(FileGUID, ProjectId);
            }


            sQLiteUtil.Execute(false, progrss1);

            return FileList.Count != 0;

        }

        public override void Execute()
        {
            Log();
        }
    }

    public class logElement
    {
        private Element ele { get; set; }
        private String elementId { get; set; }
        private string elementUniqId { get; set; }
        private string elementVersionGUID { get; set; }


        private BoundingBoxXYZ elementBbx { get; set; }

        private string elementName { get; set; }

        private string elementCategoryName { get; set; }

        private bool elementViewSpecific { get; set; }


        public string objectStatus { get; set; }



        public logElement(Element element)
        {
            ele = element;
            elementId = element.Id.ToString();
            elementUniqId = element.UniqueId;
            elementVersionGUID = element.VersionGuid.ToString();
            elementName = element.Name;
            elementCategoryName = element.Category.Name;
            elementViewSpecific = element.ViewSpecific;



            View view = null;
            if (ele.OwnerViewId != null && ele.OwnerViewId.IntegerValue != -1)
            {
                view = ele.Document.GetElement(ele.OwnerViewId) as View;
            }

            elementBbx = ele.get_BoundingBox(view);


            if (ele is Level)
            {




                Level level = ele as Level;




                View3D Collector3D = new FilteredElementCollector(ele.Document).OfClass(typeof(View3D)).WhereElementIsNotElementType().Cast<View3D>().Where(x => x.IsTemplate == false).FirstOrDefault();
                View collectorView = new FilteredElementCollector(ele.Document).OfClass(typeof(ViewPlan)).WhereElementIsNotElementType().Cast<ViewPlan>().Where(x => x.IsTemplate == false).FirstOrDefault();
                if (Collector3D == null)
                {
                    var Doc = ele.Document;
                    var direction = new XYZ(-1, 1, -1);
                    var collector = new FilteredElementCollector(Doc);
                    var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                      .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);



                    using (Transaction ttNew = new Transaction(Doc, "abc"))
                    {
                        ttNew.Start();
                        var view3D = View3D.CreateIsometric(
                                          Doc, viewFamilyType.Id);

                        view3D.SetOrientation(new ViewOrientation3D(
                          direction, new XYZ(0, 1, 1), new XYZ(0, 1, -1)));


                        ttNew.Commit();
                        elementBbx = ele.get_BoundingBox(view3D);

                    }


                }
                else
                {
                    try
                    {
                        elementBbx = ele.get_BoundingBox(ele.Document.ActiveView);
                        if (elementBbx == null)
                            elementBbx = ele.get_BoundingBox(Collector3D as View);

                    }
                    catch (Exception e)
                    {

                        TaskDialog.Show("error", e.Message);
                    }
                }


            }

        }

        public logElement(LoggedElementObject loggedElementObject)
        {

            elementUniqId = loggedElementObject.ObjectIds[0];
            elementId = loggedElementObject.ObjectIds[1];
            elementVersionGUID = loggedElementObject.ObjectIds[2];
            elementName = loggedElementObject.ObjectProperties[0];
            elementCategoryName = loggedElementObject.ObjectProperties[1];
            elementViewSpecific = loggedElementObject.ObjectProperties[2].ToLower() == "true" ? true : false;

            BoundingBoxXYZ boundingBox = new BoundingBoxXYZ();
            var bbMinStr = loggedElementObject.BBox[0][0].Replace("(", "").Replace(")", "").Split(',');
            boundingBox.Min = new XYZ(double.Parse(bbMinStr[0]), double.Parse(bbMinStr[1]), double.Parse(bbMinStr[2]));
            var bbMaxStr = loggedElementObject.BBox[1][0].Replace("(", "").Replace(")", "").Split(',');
            boundingBox.Max = new XYZ(double.Parse(bbMaxStr[0]), double.Parse(bbMaxStr[1]), double.Parse(bbMaxStr[2]));

            elementBbx = boundingBox;



        }

        public string GetLoogerText(LoggerType loggerType)
        {

            var projectNotes = Settings.Settings.ProjectNote;
            var userNotes = Settings.Settings.UserNote;
            string objectIds;

            string objectProps;
            string BBox;
            string _notes;
            string _objectStatus;
            string theLoggerText;
            if (elementBbx == null)
                return null;




            if (elementName.Contains("\""))
            {
                objectProps = $"\'ObjectProperties\': [\'{elementName}\',\'{elementCategoryName}\', \'{elementViewSpecific}\']";
                objectIds = $"\'ObjectIds\': [\'{elementUniqId}\',\'{elementId}\', \'{elementVersionGUID}\']";

                BBox = $"\'BBox\': [[\'{elementBbx.Min}\'],[\'{elementBbx.Max}\']]";


                _notes = $"\'Notes\': [\'{userNotes}\', \'{projectNotes}\']";

                _objectStatus = $"\'ObjectStatus\':\'{objectStatus}\'";

            }
            else
            {
                objectProps = $"\"ObjectProperties\": [\"{elementName}\",\"{elementCategoryName}\", \"{elementViewSpecific}\"]";
                objectIds = $"\"ObjectIds\": [\"{elementUniqId}\",\"{elementId}\", \"{elementVersionGUID}\"]";

                BBox = $"\"BBox\": [[\"{elementBbx.Min}\"],[\"{elementBbx.Max}\"]]";


                _notes = $"\"Notes\": [\"{userNotes}\", \"{projectNotes}\"]";

                _objectStatus = $"\"ObjectStatus\":\"{objectStatus}\"";
            }










            // determine if we write the change file or the log file
            if (loggerType == LoggerType.log)
            {
                theLoggerText = $"{{{objectIds}, {objectProps},{BBox}, {_notes}}}";
            }
            else
            {
                theLoggerText = $"{{{objectIds}, {objectProps},{BBox}, {_notes}, {_objectStatus}}}";
            }

            return theLoggerText;
        }


    }


    public enum LoggerType
    {
        log,
        change

    }

    public enum ElementTransactionAction
    {
        NEW,
        MODIFIED,
        DELETED
    }




}
