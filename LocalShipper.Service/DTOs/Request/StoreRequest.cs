using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class StoreRequest
    {
     
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public string StoreEmail { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public int? ZoneId { get; set; }
        public string Password { get; set; }
    }

    public class StoreRequestPut
    {

        public string StoreName { get; set; }
        public string StorePhone { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public int? Status { get; set; }

    }

    public class StoreRequestTime
    {

        public int TimeDelivery { get; set; }
    }
}
