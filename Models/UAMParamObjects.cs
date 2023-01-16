using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class ViewAccess
    {
        public int id { get; set; }
        public string menu { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public string property { get; set; }
        public IList<ViewAccess> Children { get; set; }
    }

    public class userAccess
    {
        public int id { get; set; }
        public string menu { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public int parent { get; set; }
        public string property { get; set; }
    }
}
