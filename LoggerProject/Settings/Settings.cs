using Autodesk.Revit.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RevitLogger.Settings
{
    internal static class Settings
    {

        private static bool addinActivate = false;

        public static bool AddinOnOff
        {
            get { return addinActivate; }
            set
            {
                PushButton button = HelperRevitUI.GetPushButton("Start/Stop");
                Image img1;
                ImageSource imgsc1;
                if (value)
                {
                    img1 = Properties.Resources.AppOn;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;

                }
                else
                {
                    img1 = Properties.Resources.AppOff;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;
                }
                addinActivate = value;

            }
        }
        private static bool ifcOnOff = false;
        public static bool IfcOnOff
        {
            get { return ifcOnOff; }
            set
            {
                PushButton button = HelperRevitUI.GetPushButton("IFCOnOff");
                Image img1;
                ImageSource imgsc1;
                if (value)
                {
                    img1 = Properties.Resources.IFCOn;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;

                }
                else
                {
                    img1 = Properties.Resources.IFCOff;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;
                }
                ifcOnOff = value;

            }
        }


        private static bool _settingOnOff;

        public static bool SettingOnOff
        {
            get { return _settingOnOff; }
            set
            {
                PushButton button = HelperRevitUI.GetPushButton("Settings");
                Image img1;
                ImageSource imgsc1;
                if (value)
                {
                    img1 = Properties.Resources.SettingOn;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;

                }
                else
                {
                    img1 = Properties.Resources.SettingOff;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;
                }

                _settingOnOff = value;


            }
        }

        private static bool _notesOnOff;

        public static bool NotesOnOff
        {
            get { return _notesOnOff; }
            set
            {
                PushButton button = HelperRevitUI.GetPushButton("Notes");
                Image img1;
                ImageSource imgsc1;
                if (value)
                {
                    img1 = Properties.Resources.NotesOn;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;

                }
                else
                {
                    img1 = Properties.Resources.NotesOff;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;
                }

                _notesOnOff = value;


            }
        }


        private static bool _selectFromOnOff;

        public static bool SelectFromOnOff
        {
            get { return _selectFromOnOff; }
            set
            {
                PushButton button = HelperRevitUI.GetPushButton("SelectFrom");
                Image img1;
                ImageSource imgsc1;
                if (value)
                {
                    img1 = Properties.Resources.SelectOn;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;

                }
                else
                {
                    img1 = Properties.Resources.SelectOff;
                    imgsc1 = GetImageSource(img1);
                    button.Image = imgsc1;
                    button.LargeImage = imgsc1;
                }

                _selectFromOnOff = value;


            }
        }












        public static string FullLogPath { get; set; }
        public static string LogFilterSettingPath { get; set; }
        private static string _projectName ="";

        public static string ProjectName
        {
            get
            {
                var temp = _projectName;
                if (_projectName.Contains("\'"))
                    temp = _projectName.Replace("\'", "\'\'");
                return temp;
            }
            set => _projectName = value;
        }
        public static string ProjectNumber { get; set; }
        public static string externalProjectID { get; set; }
        public static string modelName { get; set; }
        public static string modelDiscipline { get; set; }


        public static string UserNote { get; set; }
        public static string ProjectNote { get; set; }
        public static string demoLink { get; set; }
        public  static bool DeltaFileExport { get; set; }


        public static List<string> scope { get; set; }

        public static bool isModelCatFilter { get; set; }
        public static bool isAnnotationCatFilter { get; set; }
        public static bool isViewFilter { get; set; }
        public static bool isScheduleFilter { get; set; }
        public static bool isDetailFilter { get; set; }
        public static bool isSheetFilter { get; set; }
        public static bool isModelViewsFilter { get; set; }
        public static bool isLegendsFilter { get; set; }
        public static bool isFamilyTypesFilter { get; set; }
        public static bool isLinksFilter { get; set; }
        public static bool isImportedFilter { get; set; }




        public static string SaveSettingsFilePath = @"%AppData%\Magnetar\RevitLogger\";

        private static BitmapSource GetImageSource(Image img)
        {
            BitmapImage bmp = new BitmapImage();

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.UriSource = null;
                bmp.StreamSource = ms;
                bmp.EndInit();
            }
            return bmp;
        }
    }
}
