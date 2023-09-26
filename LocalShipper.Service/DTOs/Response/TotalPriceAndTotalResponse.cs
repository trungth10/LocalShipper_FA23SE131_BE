using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class TotalPriceAndTotalResponse
    {
        public decimal TotalPrice { get; set; }
        public int TotalCount { get; set; }
    }
}
