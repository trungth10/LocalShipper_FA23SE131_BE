using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IRouteService
    {

        Task<List<RouteEdgeResponse>> GetRoute(int? id, string? fromStation, string? toStation, int? quantity, int? progress, int? priority, int? status, int? shipperId, int? pageNumber, int? pageSize);

        Task<RouteEdgeResponse> AddOrderToRoute(IEnumerable<int> id, int shipperId, int routeId, double shipperLatitude,double shipperLongitude);
        Task<RouteEdgeResponse> UpdateRoute(int routeId, RouteRequest request);

        Task<MessageResponse> DeleteRoute(int routeId);
        Task<RouteEdgeResponse> CreateRoute(CreateRouteRequest request);

        Task<RouteEdgeResponse> CreateRouteSuggest(int shiperId, int money, SuggestEnum suggest, int capacityLow, int capacityHight, CreateRouteRequestAuto request, double shipperLatitude, double shipperLongitude);

        Task<List<OrderResponse>> UpdateOrderRouteId(IEnumerable<int> orderid);

        Task<(double Latitude, double Longitude)> ConvertAddress(string address);
        Task<long[,]> GetDistanceMatrix(List<(double Latitude, double Longitude)> location);
        Task<List<int>> SolveTSPAsync(long[,] distanceMatrix);

        Task<(List<int>, List<(int, int)>)> SolvePDPAsync(long[,] distanceMatrix, int[][] pickupsDeliveries);
        Task<string> ConvertLatLng(double lat, double lng);

    }
}
