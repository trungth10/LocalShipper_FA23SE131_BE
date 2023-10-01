using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutOrderRequest
    {
        public int storeId { get; set; }
        public int batchId { get; set; }
        public int shipperId { get; set; }
        public int status { get; set; }
        public string trackingNumber { get; set; }
        public DateTime? createTime { get; set; }
        public DateTime? orderTime { get; set; }
        public DateTime? acceptTime { get; set; }
        public DateTime? pickupTime { get; set; }
        public DateTime? cancelTime { get; set; }
        public string cancelReason { get; set; }
        public DateTime? completeTime { get; set; }
        public decimal? distancePrice { get; set; }
        public decimal? subTotalprice { get; set; }
        public decimal? totalPrice { get; set; }
        public string other { get; set; }
        
    }
}
