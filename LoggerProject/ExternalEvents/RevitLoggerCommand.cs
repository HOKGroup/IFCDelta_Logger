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

namespace RevitLogger.ExternalEvents
{
    public class RevitLoggerCommand : ExternalEventInfo
    {



        //Fields and Properties :
        public Document doc { get; set; }
        public UIDocument UIDocument { get; set; }
        
        public override void Execute()
        {

            //TaskDialog.Show("RevitLogger", "RevitLogger is running");
            //TaskDialog.Show("RevitLogger", "FileOpened");

          



        }


     

 



    }

   



}
