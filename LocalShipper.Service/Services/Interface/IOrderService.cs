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

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInMonth(int shipperId,int month, int year);

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInWeek(int shipperId, int month, int weekOfMonth, int year);

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInDay(int shipperId, int month, int day, int year);

        Task<List<OrderResponse>> GetOrder(int? id, int? status, int? storeId, int? batchId, int? shipperId,
            string? tracking_number, string? cancle_reason, decimal? distance_price,
            decimal? subtotal_price, decimal? totalPrice, string? other);
        Task<int> GetTotalOrderCount(int? storeId, int? shipperId);
        Task<MessageResponse> CreateOrder(OrderRequest request);
        Task<OrderResponse> UpdateOrder(int id, PutOrderRequest orderRequest);
        Task<OrderResponse> DeleteOrder(int id);
    }
}
