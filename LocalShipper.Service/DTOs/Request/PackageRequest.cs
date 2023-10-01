using MimeKit.Encodings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Request
{
    public class PackageRequest
    {
        public int BatchId { get; set; }
        public int Capacity { get; set; }
        public float PackageWeight { get; set; }
        public float PackageWidth { get; set; }
        public float PackageHeight { get; set; }
        public float PackageLength { get; set; }
        public int? Status { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public decimal DistancePrice { get; set; }
        public decimal SubtotalPrice { get; set; }

        public int ActionId { get; set; }
        public int? TypeId { get; set; }


    }
}
