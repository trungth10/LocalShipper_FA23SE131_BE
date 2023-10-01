using LocalShipper.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class BatchResponse
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string BatchName { get; set; }
        public string BatchDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public int? Status { get; set; }

        public StoreResponse Store { get; set; }
    }
}
