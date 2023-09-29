using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class RegisterHistoryRequest
    {
        public string Action { get; set; }
        public string HistoryDescription { get; set; }
        public int StoreId { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
