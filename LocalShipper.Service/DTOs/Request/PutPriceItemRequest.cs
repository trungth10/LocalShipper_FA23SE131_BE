using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutPriceItemRequest
    {
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal Price { get; set; }
        public DateTime? ApplyFrom { get; set; }
        public DateTime? ApplyTo { get; set; }
        public int PriceId { get; set; }
    }
}
