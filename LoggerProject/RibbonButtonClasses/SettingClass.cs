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

namespace RevitLogger
{
    [Transaction(TransactionMode.ReadOnly)]
    class SettingClass : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                //Getting The Active UIDocument :
                UIDocument uIDocument = commandData.Application.ActiveUIDocument;
                if (uIDocument.Document.IsFamilyDocument)
                {
                    TaskDialog.Show("Error", "Sorry this seams like family document which isn't supported");
                    return Result.Cancelled;
                }


                if (uIDocument.Document.IsFamilyDocument)
                {
                    TaskDialog.Show("Error", "Sorry this seams like family document which isn't supported");
                    return Result.Cancelled;
                }

                if (MainWindow.CurrentMainWindow == null)
                {
                    //Initializing ExternalEventHandler Event :
                    ExternalEventHandler.CreateEvent();


                    //Passing The UIDocument to The ModelView :
                    MainWindow.CurrentMainWindow = new MainWindow(uIDocument);

                    //Setting The Revit application is the owner app :
                    WindowHandler.SetWindowOwner(commandData.Application, MainWindow.CurrentMainWindow);

                    //Handling of closing Event :
                    ActiveDocumentHandler.Instance.BindWindowEvent(commandData.Application, MainWindow.CurrentMainWindow, uIDocument.Document);

                    //Showing The Window of the plugin :

                    MainWindow.CurrentMainWindow.Width = 410;
                    MainWindow.CurrentMainWindow.Height = 350;
                    MainWindow.CurrentMainWindow.ResizeMode = ResizeMode.NoResize;
                    MainWindow.CurrentMainWindow.ShowDialog();
                    if (MainWindow.IsSaved)
                    {
                    return Result.Succeeded;

                    }
                    else
                    {
                        return Result.Cancelled;
                    }
                }

                else
                {
                    MessageBox.Show("Settings Window is already open!");
                    return Result.Failed;
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", "SettingClass File\n" + e.Message);
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
