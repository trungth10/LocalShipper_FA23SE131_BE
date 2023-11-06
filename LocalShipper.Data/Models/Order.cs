using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderHistories = new HashSet<OrderHistory>();
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public int Id { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public int? ShipperId { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelReason { get; set; }
        public DateTime? CompleteTime { get; set; }
        public decimal? Distance { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Other { get; set; }
        public int? RouteId { get; set; }
        public int Capacity { get; set; }
        public int? PackageWeight { get; set; }
        public int? PackageWidth { get; set; }
        public int? PackageHeight { get; set; }
        public int? PackageLength { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCommune { get; set; }
        public string CustomerDistrict { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int ActionId { get; set; }
        public int TypeId { get; set; }
        public int? Eta { get; set; }

        public virtual PackageAction Action { get; set; }
        public virtual RouteEdge Route { get; set; }
        public virtual Shipper Shipper { get; set; }
        public virtual Store Store { get; set; }
        public virtual PackageType Type { get; set; }
        public virtual ICollection<OrderHistory> OrderHistories { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
