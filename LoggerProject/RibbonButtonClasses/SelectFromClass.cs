using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Helpers;
using Newtonsoft.Json;
using RevitLogger.Helpers;
using RevitLogger.SQLite_Helper;
using RevitLogger.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RevitLogger
{
    [Transaction(TransactionMode.ReadOnly)]
    class SelectFrom : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                //Getting The Active UIDocument :
                UIDocument uIDocument = commandData.Application.ActiveUIDocument;
                Document doc = uIDocument.Document;
                if (doc.IsFamilyDocument)
                {
                    TaskDialog.Show("Error", "Sorry this seams like family document which isn't supported");
                    return Result.Cancelled;
                }

                //list of elements unique id to be selected
                List<ElementId> elementsIdsList = new List<ElementId>();

                // Create OpenFileDialog
                OpenFileDialog dlg = new OpenFileDialog();
                // Set filter for file extension and default file extension
                dlg.DefaultExt = ".sqlite";
                dlg.Filter = "Database Files (.sqlite)|*.sqlite";
                dlg.Multiselect = true;

                // Display OpenFileDialog by calling ShowDialog method
                var result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox
                if (result == DialogResult.OK)
                {
                    var files = dlg.FileNames;
                    SQLiteUtil sQLiteUtil = SQLiteUtil.CreateSQLite();
                    var oldFullPath = sQLiteUtil._fullpath;

                    List<string> allElementsUniqueIds = new List<string>();
                    foreach (var file in files)
                    {
                        sQLiteUtil.CreateDBConnection(file);

                        var AllCurrentElementsuniqueIds = sQLiteUtil.QueryDataBase(Tables.Elements).Select(x => x[0]).Cast<string>();
                         allElementsUniqueIds = allElementsUniqueIds.Except(AllCurrentElementsuniqueIds).ToList();
                         allElementsUniqueIds = AllCurrentElementsuniqueIds.Except(allElementsUniqueIds).ToList();
         

                    }
                                                                
                    var SelectedElements = allElementsUniqueIds.Where(x => doc.GetElement(x) != null).Select(x=> doc.GetElement(x).Id).ToList();

                    uIDocument.Selection.SetElementIds(SelectedElements);

                    sQLiteUtil.CreateDBConnection(oldFullPath);

                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", "SelectFrom File\n" + e.Message);
                message = e.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        //Function to get the assembly path of the external command class :
        public static string GetAssemblyPath()
        {
            string codeBase = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var assembly = new Uri(codeBase).LocalPath;
            return assembly;
        }
    }
}
