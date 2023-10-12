using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class OrderRequest
    {
        public int Status { get; set; }
        public int StoreId { get; set; }
        public int? ShipperId { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Other { get; set; }
        public int Capacity { get; set; }
        public int? PackageWeight { get; set; }
        public int? PackageWidth { get; set; }
        public int? PackageHeight { get; set; }
        public int? PackageLength { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCommune { get; set; }
        public string CustomerDistrict { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int ActionId { get; set; }
        public int TypeId { get; set; }
    }
}
