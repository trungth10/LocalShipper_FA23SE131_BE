﻿using LocalShipper.Service.DTOs.Request;
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
        Task<List<OrderResponse>> GetOrderByShipperId(int shipperId);
    }
}
