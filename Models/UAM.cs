using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class user
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Required")]
        public string username { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Required")]
        public string fullname { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Required")]
        public string password { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Role")]
        public int role { get; set; }
        public bool isActive { get; set; }
    }

    public class role : _Base
    {
        public string roleName { get; set; }
        public string desc { get; set; }
    }

    public class menu : _Base
    {
        public string name { get; set; }
        public string action { get; set; }
        public string controller { get; set; }
        public int parent { get; set; }
        public string property { get; set; }
    }

    public class accessRight : _Base
    {
        public string role { get; set; }
        public int menu { get; set; }
    }

    public class userView
    {
        public int id { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Required")]
        public string username { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Required")]
        public string fullname { get; set; }

        [Display(Name = "Password")]        
        public string password { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Required")]
        public string email { get; set; }

        [Display(Name = "Role")]
        public string role { get; set; }
    }
}
