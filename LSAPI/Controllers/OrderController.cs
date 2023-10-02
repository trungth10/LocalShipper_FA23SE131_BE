using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using LocalShipper.Service.Helpers;

namespace LSAPI.Controllers
{
    [ApiController]
    

    //[Authorize(Policy = "Shipper")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

       
        [HttpGet("api/orders")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id, int status, int storeId, int batchId, int shipperId,
            string tracking_number, string cancle_reason, decimal distance_price,
            decimal subtotal_price, decimal totalPrice, string other)
        {
            try
            {

                var response = await _orderService.GetOrder(id,status,storeId,batchId,shipperId,tracking_number,cancle_reason,distance_price,subtotal_price,totalPrice,other);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem đơn hàng thất bại: {ex.Message}");
            }
        }

        
        [HttpGet("api/orders/count")]
        public async Task<ActionResult<OrderResponse>> GetCountOrder(int storeId, int shipperId)
        {
            try
            {

                var rs = await _orderService.GetTotalOrderCount(storeId, shipperId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

    
        [HttpPost("api/orders")]
        public async Task<ActionResult<MessageResponse>> CreateOrder(OrderRequest request)
        {
            try
            {

                var rs = await _orderService.CreateOrder(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Tạo đơn hàng thất bại: {ex.Message}");
            }

        }

        
        [HttpPut("api/orders")]
        public async Task<ActionResult<MessageResponse>> UpdateOrder(int id, PutOrderRequest orderRequest)
        {
            try
            {
                var rs = await _orderService.UpdateOrder(id, orderRequest);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đơn hàng thất bại: {ex.Message}");
            }

        }

        
        [HttpDelete("api/orders")]
        public async Task<ActionResult<MessageResponse>> DeleteOrder(int id)
        {
            try
            {
                var rs = await _orderService.DeleteOrder(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đơn hàng thất bại: {ex.Message}");
            }

        }


        [HttpPut("shipper/api/orders")]
        public async Task<ActionResult<MessageResponse>> ShipperToStatusOrder(int id, int shipperId, OrderStatusEnum status)
        {
            try
            {
                var rs = await _orderService.ShipperToStatusOrder(id, shipperId, status);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đơn hàng thất bại: {ex.Message}");
            }
        }



        /* 

         [HttpGet("totalPriceByShipperId")]
         public async Task<ActionResult<TotalPriceResponse>> GetTotalPriceSumByShipperId(int shipperId)
         {
             var rs = await _orderService.GetTotalPriceSumByShipperId(shipperId);
             return Ok(rs);
         }

         [HttpGet("CancelRateByShipperId")]
         public async Task<ActionResult<TotalPriceResponse>> GetCancelRateByShipperId(int shipperId)
         {
             var rs = await _orderService.GetCancelRateByShipperId(shipperId);
             return Ok(rs);
         }

         [HttpGet("ReceiveRateByShipperId")]
         public async Task<ActionResult<TotalPriceResponse>> GetReceiveRateByShipperId(int shipperId)
         {
             var rs = await _orderService.GetReceiveRateByShipperId(shipperId);
             return Ok(rs);
         }

         [HttpGet("TotalPriceAndOrderCountInMonth")]
         public async Task<ActionResult<(decimal TotalPrice, int TotalOrders)>> GetTotalPriceAndOrderCountInMonth(int shipperId,int month, int year)
         {
             var result = await _orderService.GetTotalPriceAndOrderCountInMonth(shipperId,month, year);
             return Ok(result);
         }

         [HttpGet("TotalPriceAndOrderCountInWeek")]
         public async Task<ActionResult<(decimal TotalPrice, int TotalOrders)>> GetTotalPriceAndOrderCountInWeek(int shipperId, int month, int weekOfMonth, int year)
         {
             var result = await _orderService.GetTotalPriceAndOrderCountInWeek(shipperId, month, weekOfMonth, year);
             return Ok(result);
         }
         [HttpGet("TotalPriceAndOrderCountInDay")]
         public async Task<ActionResult<(decimal TotalPrice, int TotalOrders)>> GetTotalPriceAndOrderCountInDay(int shipperId, int month, int day, int year)
         {
             var result = await _orderService.GetTotalPriceAndOrderCountInDay(shipperId, month, day, year);
             return Ok(result);
         }


         [HttpGet("GetCompleteOrderByShipperId")]
         public async Task<ActionResult<List<OrderResponse>>> GetCompleteOrder(int shipperId)
         {
             var orders = await _orderService.GetCompleteOrder(shipperId);
             return Ok(orders);
         }

         [HttpGet("GetCancelOrderByShipperId")]
         public async Task<ActionResult<List<OrderResponse>>> GetCancelOrder(int shipperId)
         {
             var orders = await _orderService.GetCancelOrder(shipperId);
             return Ok(orders);
         }*/
    }
}
