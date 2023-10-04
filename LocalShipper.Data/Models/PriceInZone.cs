using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class PriceInZone
    {
        public int Id { get; set; }
        public int PriceId { get; set; }
        public int ZoneId { get; set; }

        public virtual PriceL Price { get; set; }
        public virtual Zone Zone { get; set; }
    }
}
