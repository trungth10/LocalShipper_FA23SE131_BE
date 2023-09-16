using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Transport
    {
        public Transport()
        {
            Shippers = new HashSet<Shipper>();
        }

        public int Id { get; set; }
        public int TypeId { get; set; }
        public string LicencePlate { get; set; } = null!;
        public string TransportColor { get; set; } = null!;
        public string? TransportImage { get; set; }
        public string? TransportRegistration { get; set; }

        public virtual TransportType Type { get; set; } = null!;
        public virtual ICollection<Shipper> Shippers { get; set; }
    }
}
