using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class AbsenView
    {
        public int id { get; set; }
        public string idProyek { get; set; }
        public DateTime periode { get; set; }
        public List<AbsenDetailView> detail { get; set; }
        public string strDetails { get; set; }
    }

    public class AbsenDetailView
    {
        public int header { get; set; }
        public int idPersonil { get; set; }
        public string nama { get; set; }
        public string role { get; set; }
        public double mandays { get; set; }
    }

    public class AbsenParam
    {
        public string idPersonil { get; set; }
        public string Mandays { get; set; }
    }

    public class PayrollParam
    {
        public string idProyek { get; set; }
        public string judul { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }

    public class PayrollView
    {
        public string nama { get; set; }
        public string telepon { get; set; }
        public string role { get; set; }
        public double rate { get; set; }
        public double mandays { get; set; }
        public double amount { get; set; }
        public string rekening { get; set; }
    }

    public class listPayroll
    {
        public List<PayrollView> data { get; set; }
    }
}
