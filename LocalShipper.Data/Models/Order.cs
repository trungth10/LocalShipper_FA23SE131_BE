using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            Payments = new HashSet<Payment>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public int BatchId { get; set; }
        public int ShipperId { get; set; }
        public string TrackingNumber { get; set; } = null!;
        public DateTime? CreateTime { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Other { get; set; }

        public virtual Batch Batch { get; set; } = null!;
        public virtual Shipper Shipper { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
