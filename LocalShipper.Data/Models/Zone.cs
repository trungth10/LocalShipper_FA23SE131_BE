using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Zone
    {
        public Zone()
        {
            Shippers = new HashSet<Shipper>();
            Stores = new HashSet<Store>();
        }

        public int Id { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDescription { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Radius { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Shipper> Shippers { get; set; }
        public virtual ICollection<Store> Stores { get; set; }
    }
}
