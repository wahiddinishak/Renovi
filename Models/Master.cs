using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{

    public class item : _Base
    {
        [Display(Name = "Nama")]
        [Required(ErrorMessage = "Required")]
        public string nama { get; set; }

        [Display(Name = "UoM")]
        [Required(ErrorMessage = "Required")]
        public string uom { get; set; }

        [Display(Name = "Harga")]
        [Required(ErrorMessage = "Required")]
        public double harga { get; set; }
    }

    public class tukang : _Base
    {
        [Display(Name = "Nama")]
        [Required(ErrorMessage = "Required")]
        public string nama { get; set; }

        [Display(Name = "Telepon")]
        [Required(ErrorMessage = "Required")]
        public string telepon { get; set; }
        [Display(Name = "Nama Bank")]
        public string namaBank { get; set; }
        [Display(Name = "No Rekening")]
        public string rekening { get; set; }
    }
}
