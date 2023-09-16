using System;
using System.Collections.Generic;

namespace LocalShipper.Data.Models
{
    public partial class Package
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int Capacity { get; set; }
        public double? PackageWeight { get; set; }
        public double? PackageWidth { get; set; }
        public double? PackageHeight { get; set; }
        public double? PackageLength { get; set; }
        public int Status { get; set; }
        public string CustomerAddress { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string? CancelReason { get; set; }
        public int PaymentId { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal SubtotalPrice { get; set; }
        public decimal DistancePrice { get; set; }
        public int ActionId { get; set; }
        public int? TypeId { get; set; }

        public virtual PackageAction Action { get; set; } = null!;
        public virtual Batch Batch { get; set; } = null!;
        public virtual PackageType? Type { get; set; }
    }
}
