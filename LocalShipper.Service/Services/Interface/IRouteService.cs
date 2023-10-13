using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
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

        Task<List<OrderResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId);
        Task<RouteEdgeResponse> UpdateRoute(int routeId, RouteRequest request);

        Task<MessageResponse> DeleteRoute(int routeId);
    }
}
