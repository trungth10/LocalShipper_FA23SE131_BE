using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PaymentRequest
    {
        public string PaymentMethod { get; set; }
        public string PaymentImage { get; set; }
        public int PackageId { get; set; }
    }
}
