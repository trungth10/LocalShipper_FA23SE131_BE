using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class Payment
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int Status { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentImage { get; set; }
        public int PackageId { get; set; }

        public virtual Package Package { get; set; }
    }
}
