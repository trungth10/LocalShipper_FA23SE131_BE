using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class PriceLSResponse
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
        public StoreResponse Store { get; set; }
    }
}
