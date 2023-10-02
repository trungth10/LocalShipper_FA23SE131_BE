using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PutPaymentRequest
    {
        public string PaymentMethod { get; set; }
        public string PaymentImage { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public string PaymentCode { get; set; }
        public int PackageId { get; set; }

        
    }
}
