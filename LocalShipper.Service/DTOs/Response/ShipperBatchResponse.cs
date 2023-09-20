using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.DTOs.Response
{
    public class ShipperBatchResponse
    {
        public int Id { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
        public int BrandId { get; set; }
        public int DepotId { get; set; }
        public bool? Active { get; set; }
        //public StationReponse Depot { get; set; }
        //public virtual ICollection<BaseOrderReponse> Orders { get; set; }

        public ShipperBatchRouteResponse BatchRoute { get; set; }
    }

    public class ShipperBatchRouteResponse
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int DriverId { get; set; }
        public int? TotalLoads { get; set; }
        public int? TotalDistance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Capacity { get; set; }
        public virtual ICollection<ShipperRouteEdgeResponse> RouteEdges { get; set; }
    }

    public class ShipperRouteEdgeResponse
    {
        public int Id { get; set; }
        public int FromStationId { get; set; }
        public int ToStationId { get; set; }
        public int RouteId { get; set; }
        public int? Distance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //public virtual StationReponse FromStation { get; set; }
        //public virtual StationReponse ToStation { get; set; }
        //public virtual ICollection<PackageActionResponse> PackageActions { get; set; }

    }

}

