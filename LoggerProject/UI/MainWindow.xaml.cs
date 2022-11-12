using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;

namespace RevitLogger.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Fields :
        private string demoLink;
        private string projectNote;



        public UIDocument Ui;
        private Document doc { get; set; }
        public static MainWindow CurrentMainWindow { get; internal set; }
        public static bool IsSaved { get; private set; }
        public MainWindow(UIDocument uIDocument)
        {
            InitializeComponent();
            IsSaved = false;
            Ui = uIDocument;

            doc = uIDocument.Document;
            DataContext = new ViewModel(this, Ui);
            IsSaved = false;
            ReadSettings();
        }

        private void ReadSettings()
        {






            var temp = Settings.Settings.ProjectName;
            if (Settings.Settings.ProjectName.Contains("\'\'"))
            {
                temp = Settings.Settings.ProjectName.Replace("\'\'", "\'");
            }
            txtProjectName.Text = temp;
            txtProjectNumber.Text = Settings.Settings.ProjectNumber;
            txtExternalProjectId.Text = Settings.Settings.externalProjectID;
            txtModelName.Text = Settings.Settings.modelName;
            txtModleDiscipline.Text = Settings.Settings.modelDiscipline;
            CheckBoxDeltaFileExport.IsChecked = Settings.Settings.DeltaFileExport;

            txtProjectName_LostFocus(this, null);
            txtProjectNumber_LostFocus(this, null);
            txtExternalProjectId_LostFocus(this, null);
            //ImportLogPathFromXMLfile();
            //txtLogFolder.Text = Settings.Settings.LogPath;

        }

        private void ImportLogPathFromXMLfile()
        {

            string path = Settings.Settings.SaveSettingsFilePath;

            path = Environment.ExpandEnvironmentVariables(path);

            //check if the file is found
            if (!File.Exists(path + "settings.xml"))
            {

              //  Settings.Settings.LogPath = "";
                return;
            }



            // save settings XML to the directory

            using (XmlReader reader = XmlReader.Create(path + "settings.xml"))
            {
                reader.ReadStartElement("RevitLogger");
                var log = reader.ReadElementContentAsString();
               // Settings.Settings.LogPath = log;
                Settings.Settings.FullLogPath = log + $@"\{txtProjectName.Text}_{txtProjectNumber.Text}\";
                reader.Close();

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }



        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var scope = "";

            bool errorFlag = false;


            if ( txtProjectName.Text == "<enter a project name> (required)"
                || txtProjectNumber.Text == "<enter a project number> (required)")

            {
                errorFlag = true;
                MessageBox.Show("Some required fields are missing, please make sure to enter all required fields.");
            }

            if (!errorFlag)
            {
                if (demoLink == null)
                {
                    if (string.IsNullOrEmpty(Settings.Settings.demoLink))
                    {
                        demoLink = "";
                    }
                    else
                    {
                        demoLink = Settings.Settings.demoLink;
                    }

                }

                if (projectNote == null)
                {
                    projectNote = "";
                }


                var ExternalProId = (txtExternalProjectId.Text == "<enter a project external id> (optional)" || txtExternalProjectId.Text == "") ? "" : txtExternalProjectId.Text;
                List<string> projectInfoValues = new List<string>() { txtProjectName.Text, txtProjectNumber.Text, ExternalProId,txtModelName.Text,txtModleDiscipline.Text };
                List<string> revitLoggerValues = new List<string>() { scope, demoLink, projectNote, CheckBoxDeltaFileExport.IsChecked.ToString().ToLower() };

                ExtensibleStorage extensibleStorage = new ExtensibleStorage(doc, revitLoggerValues, projectInfoValues, SchemaField.both);


                ExternalEventHandler.HandlerInstance.EventInfo = extensibleStorage;
                ExternalEventHandler.ExternalEventInstance.Raise();



               // SaveXMLsettingFile();
               
               // Settings.Settings.LogPath = txtLogFolder.Text;
               // Settings.Settings.FullLogPath = txtLogFolder.Text + $@"\{txtProjectName.Text}_{txtProjectNumber.Text}\";
                Settings.Settings.ProjectNote = projectNote;
                Settings.Settings.demoLink = demoLink;
                Settings.Settings.SettingOnOff = true;
                Settings.Settings.modelName = txtModelName.Text;
                Settings.Settings.modelDiscipline = txtModleDiscipline.Text;



                IsSaved = true;
                Close();

            }


        }

        //private void SaveXMLsettingFile()
        //{

        //    string path = Settings.Settings.SaveSettingsFilePath;

        //    path = Environment.ExpandEnvironmentVariables(path);

        //    //check if the directory is found. if not we will create one
        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);


        //    // save settings XML to the directory

        //    using (XmlWriter writer = XmlWriter.Create(path + "settings.xml"))
        //    {
        //        writer.WriteStartElement("RevitLogger");
        //        writer.WriteElementString("LogPath", txtLogFolder.Text);

        //        writer.WriteEndElement();
        //        writer.Flush();
        //    }
        //}




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




        private void txtProjectName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProjectName.Text == "")
            {
                txtProjectName.Text = "<enter a project name> (required)";

                txtProjectName.Foreground = new SolidColorBrush(Color.FromRgb(159, 159, 159));

            }

        }

        private void txtProjectName_GotFocus(object sender, RoutedEventArgs e)
        {

            if (txtProjectName.Text == "<enter a project name> (required)")
            {
                txtProjectName.Text = "";
                txtProjectName.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }

        private void txtProjectNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProjectNumber.Text == "")
            {
                txtProjectNumber.Text = "<enter a project number> (required)";

                txtProjectNumber.Foreground = new SolidColorBrush(Color.FromRgb(159, 159, 159));

            }

        }

        private void txtProjectNumber_GotFocus(object sender, RoutedEventArgs e)
        {

            if (txtProjectNumber.Text == "<enter a project number> (required)")
            {
                txtProjectNumber.Text = "";
                txtProjectNumber.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }

        private void txtExternalProjectId_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtExternalProjectId.Text == "")
            {
                txtExternalProjectId.Text = "<enter a project external id> (optional)";

                txtExternalProjectId.Foreground = new SolidColorBrush(Color.FromRgb(159, 159, 159));

            }



        }

        private void txtExternalProjectId_GotFocus(object sender, RoutedEventArgs e)
        {

            if (txtExternalProjectId.Text == "<enter a project external id> (optional)")
            {
                txtExternalProjectId.Text = "";
                txtExternalProjectId.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

        }



        private void btnImportSettings_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog

            OpenFileDialog dlg = new OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV documents (.csv)|*.csv";

            // Display OpenFileDialog by calling ShowDialog method
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == System.Windows.Forms.DialogResult.OK)
            {




                // path to the csv file
                string path = dlg.FileName;


                string[] lines = System.IO.File.ReadAllLines(path);
                string[] headers = lines[0].Split(',');

                string[] columns = lines[1].Split(',');

                for (int fieldIndex = 0; fieldIndex < columns.Length; fieldIndex++)
                {


                    try
                    {

                        var value = columns[fieldIndex].Replace("\"", "");
                        switch (headers[fieldIndex].ToLower().Replace("\"", ""))
                        {


                            case "projectName":
                                txtProjectName.Text = value;
                                break;

                            case "projectnumber":
                                txtProjectNumber.Text = value;
                                break;
                            case "externalid":
                                if (value == "") break;
                                txtExternalProjectId.Text = value;
                                break;


                            case "demolink":
                                if (value == "") break;
                                demoLink = value;
                                break;

                            case "projectnote":
                                if (value == "") break;
                                projectNote = value;
                                break;

                            default:
                                break;



                        }



                    }
                    catch (Exception ex)
                    {
                        continue;

                    }

                }


            }
        }
    }
}
