using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class OrderRequest
    {
        public int storeId { get; set; }
        public int batchId { get; set; }
        public int shipperId { get; set; }
        public string cancleReason { get; set; }
    }
}
