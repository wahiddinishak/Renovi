using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace renovi.Models
{
    public class LaporanView
    {
        public int id { get; set; }
        public string idProyek { get; set; }
        public DateTime reportDate { get; set; }
        public int idActH { get; set; }
        public int idActD { get; set; }
        public string usage { get; set; }
        public string overhead { get; set; }
        public string note { get; set; }
        public double wip { get; set; }
        public List<IFormFile> fileNota { get; set; }
        public List<IFormFile> fileProgress { get; set; }
    }

    public class LaporanUsageView
    {
        public int idMaterial { get; set; }
        public string materialInfo { get; set; }
        public string materialBudget { get; set; }
        public string materialUsage { get; set; }
        public int idActD { get; set; }
        public string uom { get; set; }
        public double qty { get; set; } // koefisien analisa * volume ActD
        public double amount { get; set; } // harga satuan * volume ActD
    }

    public class LaporanUsageParam
    {
        public string idMaterial { get; set; }
        public string Kuantitas { get; set; } // koefisien analisa * volume ActD
        public string Biaya { get; set; } // harga satuan * volume ActD
    }

    public class listLaporanUsage
    {
        public List<LaporanUsageView> data { get; set; }
    }
    

    public class LaporanOverheadProfitView
    {
        public double budget { get; set; }
        public double usage { get; set; }
        public double amount { get; set; } // dari persentase overhead dan profit
        public string info { get; set; }
    }

    public class LaporanOverheadProfitParam
    {
        public string Keterangan { get; set; }
        public string Biaya { get; set; }
    }


    public class LaporanAttachmentView
    {
        public int id { get; set; }
        public string idProyek { get; set; }
        public int header { get; set; }
        public string jenis { get; set; }
        public string filename { get; set; }
    }

    public class listLaporanAttachment
    {
        public List<LaporanAttachmentView> data { get; set; }
    }
}
