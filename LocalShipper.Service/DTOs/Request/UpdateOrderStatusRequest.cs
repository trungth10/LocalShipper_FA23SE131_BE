using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class UpdateOrderStatusRequest
    {
        public OrderStatusEnum? status { get; set; }
        public string cancelReason { get; set; }
    }
}
