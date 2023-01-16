using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class Absen : _Base
    {
        public string idProyek { get; set; }
        public DateTime periode { get; set; }
    }

    public class AbsenDetail : _Base
    {
        public int header { get; set; }
        public int idPersonil { get; set; }
        public double mandays { get; set; }
    }
}
