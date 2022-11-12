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

namespace RevitLogger.UI
{
    /// <summary>
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {




        public UIDocument Ui;
        private Document doc { get; set; }
        public static NoteWindow CurrentNoteWindow { get; internal set; }
        public NoteWindow(UIDocument uIDocument)
        {
            InitializeComponent();
            Ui = uIDocument;

            doc = uIDocument.Document;
            DataContext = new ViewModel(this, Ui);
            ReadSettings();
        }

        private void ReadSettings()
        {

            txtProjectNote.Text = Settings.Settings.ProjectNote;
            txtUserNote.Text = Settings.Settings.UserNote;
            txtUserNote_LostFocus(null, null);
            txtProjectNote_LostFocus(null, null);




        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }











        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
               var projectNote  = txtProjectNote.Text == "<enter a project note (optional)>"? "" : txtProjectNote.Text;
              var userNote = txtUserNote.Text == "<enter a user note (optional)>"  ? "" : txtUserNote.Text;
            List<string> revitLoggerValues = new List<string>() { "", "", projectNote };

            ExtensibleStorage extensibleStorage = new ExtensibleStorage(doc, revitLoggerValues, null, SchemaField.MagnetarRevitLogger);


            ExternalEventHandler.HandlerInstance.EventInfo = extensibleStorage;
            ExternalEventHandler.ExternalEventInstance.Raise();

            Settings.Settings.UserNote = userNote;
            if (projectNote != "" || userNote != "")
            {

            Settings.Settings.NotesOnOff = true;
            }
            else
            {
                Settings.Settings.NotesOnOff = false;
            }
             Close();


        }
        private void lblDemoLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(Settings.Settings.demoLink))
            {
                Process.Start(Settings.Settings.demoLink);
            }
            else
            {
                MessageBox.Show("Ooops! There is no Demo Link to navigate");
            }
        }

        private void txtUserNote_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserNote.Text == "<enter a user note (optional)>")
            {
                txtUserNote.Text = "";
                txtUserNote.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }

        private void txtUserNote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserNote.Text == "")
            {
                txtUserNote.Text = "<enter a user note (optional)>";

                txtUserNote.Foreground = new SolidColorBrush(Color.FromRgb(159, 159, 159));

            }
            else

                txtUserNote.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        }

        private void txtProjectNote_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtProjectNote.Text == "<enter a project note (optional)>")
            {
                txtProjectNote.Text = "";
                txtProjectNote.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }

        private void txtProjectNote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProjectNote.Text == "")
            {
                txtProjectNote.Text = "<enter a project note (optional)>";

                txtProjectNote.Foreground = new SolidColorBrush(Color.FromRgb(159, 159, 159));

            }
            else

                txtProjectNote.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        }
    }
}
