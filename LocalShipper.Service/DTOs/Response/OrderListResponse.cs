using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class OrderListResponse
    {
        public List<OrderResponse> Orders { get; set; }
        public int OrderCount { get; set; }
    }
}
