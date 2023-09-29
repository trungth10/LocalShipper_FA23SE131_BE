using System;
using System.Collections.Generic;

#nullable disable

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
        public string LicencePlate { get; set; }
        public string TransportColor { get; set; }
        public string TransportImage { get; set; }
        public string TransportRegistration { get; set; }
        public bool? Active { get; set; }

        public virtual TransportType Type { get; set; }
        public virtual ICollection<Shipper> Shippers { get; set; }
    }
}
