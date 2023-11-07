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

        Task<List<OrderResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId, int routeId);
        Task<RouteEdgeResponse> UpdateRoute(int routeId, RouteRequest request);

        Task<MessageResponse> DeleteRoute(int routeId);
        Task<RouteEdgeResponse> CreateRoute(CreateRouteRequest request);

        Task<List<OrderResponse>> CreateRouteSuggest(int shiperId, int money, SuggestEnum suggest, int capacityLow, int capacityHight, CreateRouteRequestAuto request);

        Task<List<OrderResponse>> UpdateOrderRouteId(IEnumerable<int> orderid);

        Task<GeocodingResponse> ConvertAddress(string address);
        Task<long[,]> GetDistanceMatrix(List<string> locations);

        Task<List<int>> SolveTSPAsync(long[,] distanceMatrix);
    }
}
