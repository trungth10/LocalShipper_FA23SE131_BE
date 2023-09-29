using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class HistoryResponse
    {
        public int Id { get; set; }
        public string Action { get; set; }
        public string HistoryDescription { get; set; }
        public int StoreId { get; set; }
        public DateTime? CreateAt { get; set; }
        public StoreResponse Store { get; set; }
    }
}
