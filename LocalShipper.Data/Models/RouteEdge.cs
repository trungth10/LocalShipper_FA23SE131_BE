using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class RouteEdge
    {
        public RouteEdge()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string? StoreId { get; set; }
        public string? ToStation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Eta { get; set; }
        public int Quantity { get; set; }
        public int? Progress { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public int ShipperId { get; set; }
      

        public virtual Shipper Shipper { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
