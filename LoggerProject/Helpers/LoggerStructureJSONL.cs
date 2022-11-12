using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitLogger.Helpers
{
    public class LoggedElementObject
    {
        public List<string> ObjectIds { get; set; }
        public List<string> ObjectProperties { get; set; }
        public List<List<string>> BBox { get; set; }
        public List<string> Notes { get; set; }
        public string ObjectStatus { get; set; }
    }
}
