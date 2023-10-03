using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Batch
    {
        public Batch()
        {
            Orders = new HashSet<Order>();
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
    }
}
