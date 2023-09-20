using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PackageRequest
    {
        public int BatchId { get; set; }
        public int PaymentId { get; set; }
        public int ActionId { get; set; }
        public int? TypeId { get; set; }


    }
}
