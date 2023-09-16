using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Rating
    {
        public int Id { get; set; }
        public int ShipperId { get; set; }
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
        public int ByStoreId { get; set; }
        public DateTime? RatingTime { get; set; }

        public virtual Store ByStore { get; set; } = null!;
        public virtual Shipper Shipper { get; set; } = null!;
    }
}
