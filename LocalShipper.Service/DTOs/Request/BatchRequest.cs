using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class BatchRequest
    {
        public int StoreId { get; set; }
        
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }
        public int? Status { get; set; }
    }
}
