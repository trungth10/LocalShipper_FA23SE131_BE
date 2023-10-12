using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            OrderHistoryFromShippers = new HashSet<OrderHistory>();
            OrderHistoryToShippers = new HashSet<OrderHistory>();
            Ratings = new HashSet<Rating>();
            RouteEdges = new HashSet<RouteEdge>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string EmailShipper { get; set; }
        public string PhoneShipper { get; set; }
        public string AddressShipper { get; set; }
        public int? TransportId { get; set; }
        public int AccountId { get; set; }
        public int? ZoneId { get; set; }
        public int? Status { get; set; }
        public string Fcmtoken { get; set; }
        public int? WalletId { get; set; }
        public int? StoreId { get; set; }
        public int Type { get; set; }

        public virtual Account Account { get; set; }
        public virtual Transport Transport { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual Zone Zone { get; set; }
        public virtual ICollection<OrderHistory> OrderHistoryFromShippers { get; set; }
        public virtual ICollection<OrderHistory> OrderHistoryToShippers { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<RouteEdge> RouteEdges { get; set; }
    }
}
