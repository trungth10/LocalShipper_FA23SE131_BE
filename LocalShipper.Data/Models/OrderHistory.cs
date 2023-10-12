using System;
using System.Collections.Generic;

#nullable disable

namespace LocalShipper.Data.Models
{
    public partial class OrderHistory
    {
        public int Id { get; set; }
        public int FromStatus { get; set; }
        public int ToStatus { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? OrderId { get; set; }
        public int? FromShipperId { get; set; }
        public int? ToShipperId { get; set; }
        public int Status { get; set; }

        public virtual Shipper FromShipper { get; set; }
        public virtual Order Order { get; set; }
        public virtual Shipper ToShipper { get; set; }
    }
}
