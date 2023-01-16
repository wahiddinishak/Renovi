using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class Laporan : _Base
    {
        public string idProyek { get; set; }
        public DateTime reportDate { get; set; }
        public int idActH { get; set; }
        public int idActD { get; set; }
        public string note { get; set; }
        public double wip { get; set; }
    }

    public class LaporanUsage : _Base
    {
        public string idProyek { get; set; }
        public int header { get; set; }
        public int idMaterial { get; set; }
        public double qty { get; set; } // koefisien analisa * volume ActD
        public double amount { get; set; } // harga satuan * volume ActD
    }

    public class LaporanOverheadProfit : _Base
    {
        public string idProyek { get; set; }
        public int header { get; set; }
        public double amount { get; set; } // dari persentase overhead dan profit
        public string info { get; set; }
    }

    public class LaporanAttachment : _Base
    {
        public string idProyek { get; set; }
        public int header { get; set; }
        public string jenis { get; set; }
        public string filename { get; set; }
    }
}
