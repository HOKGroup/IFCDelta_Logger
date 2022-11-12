using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Helpers;
using RevitLogger.ExternalEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using MessageBox = System.Windows.MessageBox;

namespace RevitLogger.UI
{
    public class ViewModel : ViewModelBase
    {
        //Fields :
        private UIDocument uIDocument;
        private Document document;
        private string xmlFilePath;
        private static int progressBarValue = 0;

        public ViewModel(Window wnd, UIDocument ui)
        {
            uIDocument = ui;
            //  document = ui.Document;
        }


        //XML File Path Property :
        public string LoggerFolderPath
        {
            get { return xmlFilePath; }
            set
            {
                xmlFilePath = value;
                OnPropertyChanged(() => LoggerFolderPath);
            }
        }

    



        //Method of Handling Closing Event (DeSelection of Elements after Closing The Plugin) :
        [BindableMethod("WhenClosed")]
        public void WhenClosed()
        {
            if (MainWindow.CurrentMainWindow != null)
                MainWindow.CurrentMainWindow.Close();

            //Male the current window is null to restart the plugin if need :
            MainWindow.CurrentMainWindow = null;




            if (NoteWindow.CurrentNoteWindow != null)
                NoteWindow.CurrentNoteWindow.Close();

            NoteWindow.CurrentNoteWindow = null;



        }

        [BindableMethod("Browse")]
        public void Browse()
        {

        }





        public override bool CanProceed(object obj)
        {
            return true;
        }

        public override void Execute()
        {
            return;
        }
    }
}
