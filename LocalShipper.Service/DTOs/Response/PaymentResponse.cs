using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int Status { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentImage { get; set; }
        public int PackageId { get; set; }
        public PackageResponse Package { get; set; }
    }
}
