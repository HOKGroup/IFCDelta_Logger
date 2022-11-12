using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Helpers;
using RevitLogger.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BIM.IFC.Export.UI;

namespace RevitLogger
{
    [Transaction(TransactionMode.Manual)]
    class ExportIFCClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                //Getting The Active UIDocument :
                UIDocument uIDocument = commandData.Application.ActiveUIDocument;
                UIApplication uIApplication = commandData.Application;
                if (uIDocument.Document.IsFamilyDocument)
                {
                    TaskDialog.Show("Error", "Sorry this seams like family document which isn't supported");
                    return Result.Cancelled;
                }


                Settings.Settings.IfcOnOff = !Settings.Settings.IfcOnOff;
                if (Logger.uIApplication == null)
                    Logger.uIApplication = commandData.Application;


                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", "NoteClass File\n" + e.Message);
                message = e.Message;
                return Result.Failed;
            }

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
