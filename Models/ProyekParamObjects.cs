using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{

    
    public class ProyekInfoParam
    {
        public int Id { get; set; }
        public string idProyek { get; set; }

        [Display(Name = "Nama Proyek")]
        [Required(ErrorMessage = "Required")]
        public string judul { get; set; }

        [Display(Name = "Alamat")]
        [Required(ErrorMessage = "Required")]
        public string Alamat { get; set; }

        [Display(Name = "Mandor")]
        [Required(ErrorMessage = "Required")]
        public int mandor { get; set; }

        [Display(Name = "Klien")]
        [Required(ErrorMessage = "Required")]
        public string namaKlien { get; set; }
        public DateTime tglMulai { get; set; }
        public DateTime tglSelesai { get; set; }

        [Display(Name = "Kontrak")]
        public IFormFile kontrak { get; set; }

        [Display(Name = "Desain")]
        public IFormFile desain { get; set; }
    }

    public class ProyekView
    {
        public int Id { get; set; }
        public string idProyek { get; set; }
        public string judul { get; set; }
        public string Alamat { get; set; }
        public string mandor { get; set; }
        public string namaKlien { get; set; }
        public DateTime tglMulai { get; set; }
        public DateTime tglSelesai { get; set; }
        public string kontrak { get; set; }
        public string desain { get; set; }
        public bool isActive { get; set; }
    }

    public class listProyek
    {
        public List<ProyekView> data { get; set; }
        
    }

    public class upload
    {
        public IFormFile data { get; set; }
        public string idProyek { get; set; }
    }

    public class ActView
    {
        public int id { get; set; }        
        public string idProyek { get; set; }
        public int Seq { get; set; }
        public string kegiatan { get; set; }
        public double total { get; set; }
        public List<ActDView> details { get; set; }
    }

    public class ActDView
    {
        public int Id { get; set; }
        public int header { get; set; }
        public string kegiatan { get; set; }
        public double volume { get; set; }
        public string uom { get; set; }
        public double profit { get; set; }
        public double harga { get; set; }
    }

    public class ActDParam
    {
        public string Id { get; set; }
        public string Kegiatan { get; set; }
        public string Satuan { get; set; }
        public double Volume { get; set; }        
    }

    public class listAction
    {
        public List<ActView> data { get; set; }
    }


    public class ImportPekerjaanHeader
    {
        public string id { get; set; }
        public string idProyek { get; set; }
        public int Seq { get; set; }
        public string kegiatan { get; set; }
    }

    public class ImportPekerjaanDetail
    {
        public string Id { get; set; }
        public string header { get; set; }
        public string kegiatan { get; set; }
        public double volume { get; set; }
        public string satuan { get; set; }
    }

    public class MatView
    {
        public int id { get; set; }
        public string jenis { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public double harga { get; set; }
    }


    public class MatParam
    {
        public string id { get; set; }
        public string item { get; set; }
        public string satuan { get; set; }
        public double harga { get; set; }
    }

    public class listMat
    {
        public List<MatView> data { get; set; }
    }


    public class analisaView
    {
        public int id { get; set; }
        public int idActD { get; set; }
        public string jenisItem { get; set; }

        [Display(Name = "Item")]
        public string item { get; set; }

        [Display(Name = "Satuan")]
        public string satuan { get; set; }

        [Display(Name = "Harga")]
        public double harga { get; set; }

        [Display(Name = "Koefisien")]
        public double koefisien { get; set; }
        public double jumlah { get; set; }
    }


    public class analisaParam
    {
        public int id { get; set; }
        public int idActD { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Required")]
        public int idMaterial { get; set; }

        [Display(Name = "Koefisien")]
        [Required(ErrorMessage = "Required")]
        public double koefisien { get; set; }
    }

    public class listAnalisa
    {
        public List<analisaView> data { get; set; }
    }

    public class PersonilView
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public string Telepon { get; set; }
        public string Rekening { get; set; }
        public string Role { get; set; }
    }

    public class listTukang
    {
        public List<PersonilView> data { get; set; }
    }

    public class ActForJadwalView
    {
        public int id { get; set; }
        public string idProyek { get; set; }
        public int Seq { get; set; }
        public string kegiatan { get; set; }
        public double total { get; set; }
        public List<ActDForJadwalView> details { get; set; }
    }

    public class ActDForJadwalView
    {
        public int Id { get; set; }
        public int originalId { get; set; }
        public string blocked { get; set; }
        public int header { get; set; }
        public string kegiatan { get; set; }
    }

    public class listActionForJadwal
    {
        public List<ActForJadwalView> data { get; set; }
    }

    public class jadwalParam
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public string Blocked { get; set; }
    }


    public class rekapMaterialActHView
    {
        public int nomor { get; set; }
        public string kegiatan { get; set; }
        public List<rekapMaterialActDView> details { get; set; }
    }

    public class rekapMaterialActDView
    {
        public int nomor { get; set; }
        public string kegiatan { get; set; }
        public List<rekapMaterialItemView> items { get; set; }
    }
    public class rekapMaterialItemView
    {
        public int idMaterial { get; set; }
        public int idActD { get; set; }
        public string item { get; set; }
        public string uom { get; set; }
        public double qtyBudget { get; set; }
        public double amountBudget { get; set; }
        public double qtyUsage { get; set; }
        public double amountUsage { get; set; }
    }

    public class usageData 
    {
        public double qty { get; set; }
        public double amount { get; set; }
    }

    public class rekapMaterialView
    {
        public string judul { get; set; }
        public string alamat { get; set; }
        public string pemilik { get; set; }
        public string period { get; set; }
        public List<rekapMaterialActHView> ActMat { get; set; }
    }

    public class listRekapMaterial
    {
        public List<rekapMaterialView> data { get; set; }
    }

    public class rekapWipView
    {
        public string judul { get; set; }
        public string alamat { get; set; }
        public string pemilik { get; set; }
        public string period { get; set; }
        public List<rekapWipActHView> ActWip { get; set; }
    }

    public class rekapWipActHView
    {
        public int nomor { get; set; }
        public string kegiatan { get; set; }
        public double totalJumlahHarga { get; set; }
        public double totalBobot { get; set; }
        public double totalWip { get; set; }
        public double totalAmount { get; set; }
        public List<rekapWipActDView> details { get; set; }
    }
    public class rekapWipActDView
    {
        public int nomor { get; set; }
        public string kegiatan { get; set; }
        public string satuan { get; set; }
        public double volume { get; set; }
        public double hargaSatuan { get; set; }
        public double jumlahHarga { get; set; }
        public double bobot { get; set; }
        public double wip { get; set; }
        public double amount { get; set; }
    }

    public class listRekapWip
    {
        public List<rekapWipView> data { get; set; }
    }

    public class rekapView
    {
        public string judul { get; set; }
        public string alamat { get; set; }
        public string pemilik { get; set; }
        public string period { get; set; }
        public double totalNilai { get; set; }
        public double totalBobot { get; set; }
        public double totalProgressNilaiPerPeriod { get; set; }
        public double totalProgressBobotPerPeriod { get; set; }
        public double totalProgressNilaiOverall { get; set; }
        public double totalProgressBobotOverall { get; set; }
        public List<rekapDView> details { get; set; }
    }

    public class rekapDView
    {
        public int no { get; set; }
        public string pekerjaan { get; set; }
        public double nilai { get; set; }
        public double bobot { get; set; }
        public double progressNilaiPerPeriod { get; set; }
        public double progressBobotPerPeriod { get; set; }
        public double progressNilaiOverall { get; set; }
        public double progressBobotOverall { get; set; }
    }

    public class listRekap
    {
        public List<rekapView> data { get; set; }
    }

    public class periodRekap
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
    }

    public class ArsipView
    {
        public int idReport { get; set; }
        public DateTime reportDate { get; set; }
        public double nilai { get; set; }
        public double bobot { get; set; }
    }

    public class listArsip
    {
        public List<ArsipView> data { get; set; }
        public double totalNiali { get; set; }
        public double totalBobot { get; set; }
        public string judulProyek { get; set; }
        public string pekerjaan { get; set; }
        public string detailPekerjaan { get; set; }
        public int aiProyek { get; set; }
    }


    public class ActHTemp
    {
        public int idActH_Old { get; set; }
        public int idActH_New { get; set; }
    }

    public class ActDTemp
    {
        public int idActD_Old { get; set; }
        public int idActD_New { get; set; }
    }

    public class MaterialTemp
    {
        public int idMaterial_Old { get; set; }
        public int idMaterial_New { get; set; }
    }
}
