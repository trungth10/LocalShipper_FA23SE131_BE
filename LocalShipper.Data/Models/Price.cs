using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Price
    {
        public Price()
        {
            PriceInZones = new HashSet<PriceInZone>();
            PriceItems = new HashSet<PriceItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? PriceInZoneId { get; set; }
        public int? StoreId { get; set; }
        public int? Hourfilter { get; set; }
        public int? Datefilter { get; set; }
        public int Mode { get; set; }
        public int Status { get; set; }

        public virtual Store Store { get; set; }
        public virtual ICollection<PriceInZone> PriceInZones { get; set; }
        public virtual ICollection<PriceItem> PriceItems { get; set; }
    }
}
