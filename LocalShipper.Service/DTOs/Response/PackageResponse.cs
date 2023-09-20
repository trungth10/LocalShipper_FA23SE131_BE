using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class PackageResponse
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int Capacity { get; set; }
        public double? PackageWeight { get; set; }
        public double? PackageWidth { get; set; }
        public double? PackageHeight { get; set; }
        public double? PackageLength { get; set; }
        public int Status { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CancelReason { get; set; }
        public int PaymentId { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal SubtotalPrice { get; set; }
        public decimal DistancePrice { get; set; }
        public int ActionId { get; set; }
        public int? TypeId { get; set; }
    }
}
