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
    class NotesClass : IExternalCommand
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

                if (NoteWindow.CurrentNoteWindow == null)
                {
                    //Initializing ExternalEventHandler Event :
                    ExternalEventHandler.CreateEvent();


                    //Passing The UIDocument to The ModelView :
                    NoteWindow.CurrentNoteWindow = new NoteWindow(uIDocument);

                    //Setting The Revit application is the owner app :
                    WindowHandler.SetWindowOwner(commandData.Application, NoteWindow.CurrentNoteWindow);

                    //Handling of closing Event :
                    ActiveDocumentHandler.Instance.BindWindowEvent(commandData.Application, NoteWindow.CurrentNoteWindow, uIDocument.Document);

                    //Showing The Window of the note  :

                    NoteWindow.CurrentNoteWindow.Show();
                    NoteWindow.CurrentNoteWindow.Width = 400;
                    NoteWindow.CurrentNoteWindow.Height = 250;
                    NoteWindow.CurrentNoteWindow.ResizeMode = ResizeMode.NoResize;
                    return Result.Succeeded;
                }

                else
                {
                    MessageBox.Show("The Notes is already running");
                    return Result.Failed;
                }
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
