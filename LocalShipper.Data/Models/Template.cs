using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Template
    {
        public Template()
        {
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
    }
}
