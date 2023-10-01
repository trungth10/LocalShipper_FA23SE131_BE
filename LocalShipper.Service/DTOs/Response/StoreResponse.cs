using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class StoreResponse
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public string StorePhone { get; set; }
        public string StoreEmail { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string StoreDescription { get; set; }
        public int? Status { get; set; }
        public int BrandId { get; set; }
        public int? TemplateId { get; set; }
        public int? ZoneId { get; set; }
        public int WalletId { get; set; }
        public int AccountId { get; set; }
    }
}
