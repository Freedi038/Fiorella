using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Models
{
    public class PageSign
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [StringLength(100)]
        public string Desc { get; set; }
        [MaxLength(255)]
        public string Signature { get; set; }
    }
}
