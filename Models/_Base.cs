using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace renovi.Models
{
    public class _Base
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AppUserCreatedBy")]
        public int CreateBy { get; set; }
        public virtual user AppUserCreatedBy { get; set; }

        public DateTime? CreateDate { get; set; }

        [ForeignKey("AppUserModifedBy")]
        public int? UpdateBy { get; set; }
        public virtual user AppUserModifedBy { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
