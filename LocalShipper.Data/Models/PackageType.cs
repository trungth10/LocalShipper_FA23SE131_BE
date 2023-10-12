using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class PackageType
    {
        public PackageType()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string PackageType1 { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
