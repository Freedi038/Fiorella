using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrontToBack.Models
{
    public class Slide
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [NotMapped, Required]
        public IFormFile File { get; set; }

        [NotMapped, Required]
        public IFormFile[] Files { get; set; }
    }
}
