using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class RouteEdgeResponse
    {
        public int Id { get; set; }
        public int? FromStation { get; set; }
        public int? ToStation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? Eta { get; set; }
        public int Quantity { get; set; }
        public int? Progress { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public int ShipperId { get; set; }
        public ShipperResponse Shipper { get; set; }
    }
}
