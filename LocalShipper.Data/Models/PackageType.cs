using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class PackageType
    {
        public PackageType()
        {
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public int PackageId { get; set; }
        public string PackageType1 { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
