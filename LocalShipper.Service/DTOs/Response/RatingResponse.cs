using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class RatingResponse
    {
        public int Id { get; set; }
        public int ShipperId { get; set; }
        public string ShipperName { get; set; }
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public int ByStoreId { get; set; }
        public DateTime? RatingTime { get; set; }
        public string NameStore { get; set; }

        public ShipperResponse Shipper { get; set; }
        public StoreResponse Store { get; set; }

    }
}
