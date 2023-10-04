using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PriceLSRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? StoreId { get; set; }
        public int? Hourfilter { get; set; }
        public int? Datefilter { get; set; }
        public int Mode { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
