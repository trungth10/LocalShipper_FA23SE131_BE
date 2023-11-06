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
        public decimal? Distance { get; set; }
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
        public int? Eta { get; set; }
    }

    public class OrderRequestForCreate
    {

        public int? StoreId { get; set; }      
        public decimal SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public string Other { get; set; }
        public DateTime? OrderTime { get; set; }
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
        public int? Eta { get; set; }
    }
    public class OrderRequestForUpdate
    {
      
        public int? ShipperId { get; set; }
        public decimal Distance { get; set; }
        public decimal DistancePrice { get; set; }
        public decimal SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public string Other { get; set; }
        public DateTime? OrderTime { get; set; }
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
        public int? Eta { get; set; }
    }

    public class OrderRequestV2
    {
        public IEnumerable<int>? id {get ; set;}
        public IEnumerable<int>? status { get; set; }
        public IEnumerable<int>? storeId { get; set; }
        public IEnumerable<int>? shipperId { get; set; }
        public IEnumerable<string>? tracking_number { get; set; }
        public IEnumerable<string>? cancel_reason { get; set; }
        public IEnumerable<string>? other { get; set; }
        public int? capacity { get; set; } = 0;
        public decimal? COD { get; set; } = 0;
        public decimal? Distance { get; set; } = 0;
        public IEnumerable<int>? routeId { get; set; }    
        public IEnumerable<string>? customer_city { get; set; }
        public IEnumerable<string>? customer_commune { get; set; }
        public IEnumerable<string>? customer_district { get; set; }
        public IEnumerable<string>? customer_phone { get; set; }
        public IEnumerable<string>? customer_name { get; set; }
        public IEnumerable<string>? customer_email { get; set; }
        public IEnumerable<int>? actionId { get; set; }
        public IEnumerable<int>? typeId { get; set; }
        public int? Eta { get; set; }
    }
}
