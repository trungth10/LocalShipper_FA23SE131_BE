using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public int? ShipperId { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelReason { get; set; }
        public DateTime? CompleteTime { get; set; }
        public decimal? Distance { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Other { get; set; }
        public int? RouteId { get; set; }
        public int Capacity { get; set; }
        public int? PackageWeight { get; set; }
        public int? PackageWidth { get; set; }
        public int? PackageHeight { get; set; }
        public int? PackageLength { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerCommune { get; set; }
        public string CustomerDistrict { get; set; }

        public double CustomerLat { get; set; }
        public double CustomerLng { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int ActionId { get; set; }
        public int TypeId { get; set; }
        public int? Eta { get; set; }
        public List<string> SortedAddresses { get; set; }

        public StoreResponse Store {get; set;}
        public ShipperResponse Shipper { get; set;}
        public PackageActionResponse PackageAction { get; set;}
        public PackageTypeResponse PackageType { get; set;}
        public RouteEdgeResponse RouteEdge { get; set;}

    }
    public class OrderCreateResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public int? ShipperId { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelReason { get; set; }
        public DateTime? CompleteTime { get; set; }
        public decimal? Distance { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Other { get; set; }
        public int? RouteId { get; set; }
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

        public StoreResponse Store { get; set; }
        public ShipperResponse Shipper { get; set; }
        public PackageActionResponse PackageAction { get; set; }
        public PackageTypeResponse PackageType { get; set; }
        public RouteEdgeResponse RouteEdge { get; set; }
    }
    public class OrderWithShipperResponse {
        public int Id { get; set; }
        public int Status { get; set; }
        public int StoreId { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? CancelTime { get; set; }
        public string CancelReason { get; set; }
        public DateTime? CompleteTime { get; set; }
        public decimal? Distance { get; set; }
        public decimal? DistancePrice { get; set; }
        public decimal? SubtotalPrice { get; set; }
        public decimal? Cod { get; set; }
        public decimal? TotalPrice { get; set; }
        public string Other { get; set; }
        public int? RouteId { get; set; }
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

    public class TSPResponse
    {
        public int RouteId { get; set; }
        public int ShipperId { get; set; }
        public List<string> SortedAddresses { get; set; }

    }


}
