using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RegisterRatingRequest
    {
        public int ShipperId { get; set; }
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public int ByStoreId { get; set; }
        public DateTime? RatingTime { get; set; }
    }
}
