using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{

    public class showAddButton : _Base
    {
        public bool show { get; set; }
    }

    public class Proyek : _Base
    {
        public string idProyek { get; set; }
        public string judul { get; set; }
        public string alamat { get; set; }
        public int mandor { get; set; }
        public string namaKlien { get; set; }
        public DateTime tglMulai { get; set; }
        public DateTime tglSelesai { get; set; }
        public string kontrak { get; set; }
        public string desain { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
    }

    public class ActH : _Base
    {
        public string idProyek { get; set; }
        public int Seq { get; set; }
        public string kegiatan { get; set; }
    }

    public class ActD : _Base
    {
        public string idProyek { get; set; }
        public int header { get; set; }
        public string kegiatan { get; set; }
        public double volume { get; set; }
        public string uom { get; set; }
        public double profit { get; set; } // di dapat saat isi analisa
    }

    public class analisa : _Base
    {
        public int idActD { get; set; }
        public int idMaterial { get; set; }
        public double koefisien { get; set; }
    }

    public class Material : _Base
    {
        public string idProyek { get; set; }
        public string jenis { get; set; }
        public string item { get; set; }
        public string uom { get; set; }
        public double harga { get; set; }
    }

    public class Personil : _Base
    {
        public string idProyek { get; set; }

        [Display(Name = "Nama")]
        [Required(ErrorMessage = "Required")]
        public string nama { get; set; }
        [Display(Name = "Telepon")]
        [Required(ErrorMessage = "Required")]
        public string telepon { get; set; }
        [Display(Name = "Rekening")]
        [Required(ErrorMessage = "Required")]
        public string akunBank { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "Required")]
        public int role { get; set; } // get from material => jasa
    }

    public class Jadwal : _Base
    {
        public string idProyek { get; set; }
        public int idActD { get; set; }
        public int header { get; set; }
        public string blocked { get; set; }
    }

    public class Tagihan : _Base
    {
        public string idProyek { get; set; }
        public int Seq { get; set; }
        public DateTime tglBayar { get; set; }
        public double nominal { get; set; }
        public bool isPaid { get; set; }
    }
}
