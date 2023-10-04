using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PriceInZoneRequest
    {
        public int Id { get; set; }
        public int PriceId { get; set; }
        public int ZoneId { get; set; }

    }
}
