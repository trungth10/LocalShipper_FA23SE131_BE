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
    public interface IOrderService
    {
        Task<OrderResponse> ShipperToStatusOrder(int id, int shipperId, string? cancelReason, OrderStatusEnum status);

        Task<decimal> GetTotalPriceSumByShipperId(int shipperId);

        Task<decimal> GetCancelRateByShipperId(int shipperId);
        Task<decimal> GetReceiveRateByShipperId(int shipperId);
        Task<List<OrderResponse>> GetOrder(int? id, int? status, int? storeId, int? shipperId,
                                      string? tracking_number, string? cancel_reason, decimal? distance_price,
                                      decimal? subtotal_price, decimal? COD, decimal? totalPrice, string? other, int? routeId,
                                      int? capacity, int? package_weight, int? package_width, int? package_height, int? package_length,
                                      string? customer_city, string? customer_commune, string? customer_district, string? customer_phone,
                                      string? customer_name, string? customer_email, int? actionId, int? typeId, int? pageNumber, int? pageSize);
        Task<int> GetTotalOrderCount(int? storeId, int? shipperId);
        Task<OrderResponse> DeleteOrder(int id);
        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCount(int shipperId, int? month, int? year, int? day);

        Task<OrderResponse> CreateOrder(OrderRequestForCreate request);
        Task<OrderResponse> UpdateOrder(int id, OrderRequestForUpdate request);

    }
}
