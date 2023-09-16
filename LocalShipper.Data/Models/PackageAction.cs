using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class PackageAction
    {
        public PackageAction()
        {
            Packages = new HashSet<Package>();
        }

        public int Id { get; set; }
        public int? PackageId { get; set; }
        public int? ActionType { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Package> Packages { get; set; }
    }
}
