using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Package
    {
        public Package()
        {
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public int BatchId { get; set; }
        public int Capacity { get; set; }
        public double? PackageWeight { get; set; }
        public double? PackageWidth { get; set; }
        public double? PackageHeight { get; set; }
        public double? PackageLength { get; set; }
        public int Status { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CancelReason { get; set; }
        public decimal SubtotalPrice { get; set; }
        public decimal DistancePrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int ActionId { get; set; }
        public int? TypeId { get; set; }

        public virtual PackageAction Action { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual PackageType Type { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
