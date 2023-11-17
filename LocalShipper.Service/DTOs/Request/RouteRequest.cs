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
       
        public int? Status { get; set; }

    }
    public class CreateRouteRequest
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public int ShipperId { get; set; }


    }

    public class CreateRouteRequestAuto
    {
        public DateTime? StartDate { get; set; }
    }
}
