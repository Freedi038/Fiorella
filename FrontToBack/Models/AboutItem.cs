using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Models
{
    public class AboutItem
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public int AboutId { get; set; }


        public About About { get; set; }
    }
}
