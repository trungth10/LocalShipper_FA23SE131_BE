using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class TotalPriceResponse
    {
        public int orderId { get; set; }
        public DateTime? CompleteTime { get; set; }

        public decimal? distancePrice { get; set; }
        public decimal? subTotalprice { get; set; }
        public decimal? totalPrice { get; set; }

    }
}
