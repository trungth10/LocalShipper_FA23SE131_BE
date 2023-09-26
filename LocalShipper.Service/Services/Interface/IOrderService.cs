using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IOrderService
    {
        Task<OrderResponse> UpdateShipperInOrder(int orderId, OrderRequest request);
        Task<OrderResponse> CompleteOrder(int orderId, UpdateOrderStatusRequest request);
        Task<OrderResponse> PickupProduct(int orderId, UpdateOrderStatusRequest request);
        Task<OrderResponse> CancelOrder(int orderId, UpdateOrderStatusRequest request);
        Task<OrderResponse> GetOrderById(int id);
        Task<List<OrderResponse>> GetOrdersByAssigning();
        Task<OrderListResponse> GetOrderByShipperId(int shipperId);
        Task<TotalPriceResponse> GetTotalPriceByOrderId(int orderId);

        Task<decimal> GetTotalPriceSumByShipperId(int shipperId);

        Task<decimal> GetCancelRateByShipperId(int shipperId);
        Task<decimal> GetReceiveRateByShipperId(int shipperId);

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInMonth(int shipperId,int month, int year);

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInWeek(int shipperId, int month, int weekOfMonth, int year);

        Task<TotalPriceAndTotalResponse> GetTotalPriceAndOrderCountInDay(int shipperId, int month, int day, int year);
    }
}
