using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class PackageAction
    {
        public PackageAction()
        {
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public string ActionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Deleted { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
