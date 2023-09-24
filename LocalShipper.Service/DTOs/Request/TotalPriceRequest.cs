using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class TotalPriceRequest
    {
        public int shipperId { get; set; }
        public DateTime month { get; set; }
        public DateTime year { get; set; }

    }
}
