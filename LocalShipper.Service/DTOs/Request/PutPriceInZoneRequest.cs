using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutPriceInZoneRequest
    {
        public int priceId { get; set; }
        public int zoneId { get; set; }
    }
}
