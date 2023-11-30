using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IOrderService
    {
        Task<OrderResponse> ShipperToStatusOrder(int id, int shipperId, string? cancelReason, OrderStatusEnum status, int? routesId);

        Task<decimal> GetTotalPriceSumByShipperId(int shipperId);

        Task<decimal> GetCancelRateByShipperId(int shipperId);
        Task<decimal> GetReceiveRateByShipperId(int shipperId);
        Task<List<OrderResponse>> GetOrder(int? zoneId, int? id, int? status, int? storeId, int? shipperId,
                                     string? tracking_number, string? cancel_reason, decimal? distance, decimal? distance_price,
                                     decimal? subtotal_price, decimal? COD, decimal? totalPrice, string? other, int? routeId,
                                     int? capacity, int? package_weight, int? package_width, int? package_height, int? package_length,
                                     string? customer_city, string? customer_commune, string? customer_district, string? customer_phone,
                                     string? customer_name, string? customer_email, int? actionId, int? typeId, int? pageNumber, int? pageSize, double? shipperLatitude, double? shipperLongitude);
        Task<int> GetTotalOrderCount(int? storeId, int? shipperId);
        Task<OrderResponse> DeleteOrder(int id);
        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCount(int shipperId, int? month, int? year, int? day);

        Task<OrderCreateResponse> CreateOrder(OrderRequestForCreate request);
        Task<OrderResponse> UpdateOrder(int id, OrderRequestForUpdate request);
        Task<List<OrderResponse>> GetOrderSuggest(int id, SuggestEnum suggest, int money);

        Task<List<OrderResponse>> GetOrderV2(OrderRequestV2 request, int? pageNumber, int? pageSize);
        Task<decimal> ConvertAddressToPrice(string addressStore, string address, int storeId);
        Task<OrderResponse> GetOrderByCus(int id);

        Task<string> UploadEvidence(int orderId, IFormFile evidence);
        Task CheckDeliveryStatus(int id, int timeRequest);

    }
}
