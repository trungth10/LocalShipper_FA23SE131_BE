using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class PriceItem
    {
        public int Id { get; set; }
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal Price { get; set; }
        public DateTime? ApplyFrom { get; set; }
        public DateTime? ApplyTo { get; set; }
        public int PriceId { get; set; }

        public virtual PriceL PriceNavigation { get; set; }
    }
}
