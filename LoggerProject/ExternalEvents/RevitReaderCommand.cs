using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Windows;
using RevitLogger.Helpers;

namespace RevitLogger.ExternalEvents
{
    public class RevitReaderCommand : ExternalEventInfo
    {



        //Fields and Properties :
        public Document doc { get; set; }
        public UIDocument UIDocument { get; set; }

        public override void Execute()
        {

            var text = File.ReadAllLines("AhmedjsonL.txt");
            foreach (var line in text)
            {
                try
                {

                    LoggedElementObject myDeserializedClass = JsonConvert.DeserializeObject<LoggedElementObject>(line);
                }
                catch (Exception e)
                {

                         TaskDialog.Show("error", e.Message);
                }

            }

            TaskDialog.Show("", "Heeey");

        }









    }
}
