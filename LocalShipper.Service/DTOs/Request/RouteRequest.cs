using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RouteRequest
    {
        public string Name { get; set; }
        public string? FromStation { get; set; }
        public string? ToStation { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Eta { get; set; }
        public int? Quantity { get; set; }
        public int? Progress { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public int ShipperId { get; set; }
       

    }
    public class CreateRouteRequest
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public int ShipperId { get; set; }


    }
}
