using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RevitLogger.Settings;
using System.Diagnostics;
using System.Windows.Forms;

using System.Xml;
using MessageBox = System.Windows.MessageBox;
using Autodesk.Revit.DB;
using Color = System.Windows.Media.Color;
using System.IO;
using Helpers;
using RevitLogger.Helpers;

namespace RevitLogger.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {

       
        //Fields :
        private string demoLink;
        private Document _document;
        private bool _firstSave = false;
        public static ProgressWindow CurrentMainWindow { get; internal set; }
        public ProgressWindow(Document document, bool FirstSave)
        {
            _document = document;
            _firstSave = FirstSave;
            InitializeComponent();

         

            // DataContext = new ViewModel(this,  Ui);

            Globals.progressBarValue = 0;
          

        }



        private void Window_Initialized(object sender, EventArgs e)
        {

           

        }

        public void UpdateProgressBarValue()
        {
          
            prgbar.Value = Globals.progressBarValue;

            if (Globals.progressBarValue > 99)
            {
                //Close();
                //prgbar.Value = 0;
                //Globals.progressBarValue = 0;
            }
        }


        private void logElementProgress(bool firstSave)
        {
         
                Guid id = Guid.NewGuid();
                String sID = id.ToString();
                if (Logger.modelGuid == null)
                    Logger.modelGuid = sID;
            //    Logger logger = new Logger( this ,_document, firstSave);
            //ExternalEventHandler.CreateEvent();
            //ExternalEventHandler.HandlerInstance.EventInfo = logger;
            //ExternalEventHandler.ExternalEventInstance.Raise();



        }

        private void prgbar_Loaded(object sender, RoutedEventArgs e)
        {
           
            //Extract.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            logElementProgress(_firstSave);
            prgbar.Value += 20;
        }
    }
}
