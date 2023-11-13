using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class ShipperResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public string EmailShipper { get; set; }
        public string PhoneShipper { get; set; }
        public string AddressShipper { get; set; }
        public int TransportId { get; set; }
        public int AccountId { get; set; }
        public int ZoneId { get; set; }
        public ShipperStatusEnum Status { get; set; }
        public string Fcmtoken { get; set; }
        //public OrderResponse Order { get; set; }
        public TransportResponse Transport { get; set; }
        public AccountResponse Account { get; set; }
        public ZoneResponse Zone { get; set; }
        public List<RouteEdgeWithShipperResponse> Route { get; set; }
        public List<OrderWithShipperResponse> OrdersInRoute { get; set; }
        

    }
}
