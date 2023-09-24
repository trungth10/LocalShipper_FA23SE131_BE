using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Policy = "Shipper")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPut("{orderId:int}/ShipperAcpOrder")]
        public async Task<ActionResult<OrderResponse>> UpdateShipperInOrder(int orderId, [FromBody] OrderRequest request)
        {
            try
            {
              
                var response = await _orderService.UpdateShipperInOrder(orderId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật người giao hàng trong hóa đơn thất bại: {ex.Message}");
            }
        }

        [HttpPut("{orderId:int}/CompleteOrder")]
        public async Task<ActionResult<OrderResponse>> CompleteOrder(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {

                var response = await _orderService.CompleteOrder(orderId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật hoàn thành đơn hàng thất bại: {ex.Message}");
            }
        }

        [HttpPut("{orderId:int}/PickupProduct")]
        public async Task<ActionResult<OrderResponse>> PickupProduct(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {

                var response = await _orderService.PickupProduct(orderId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Nhận đơn hàng thất bại: {ex.Message}");
            }
        }

        [HttpPut("{orderId:int}/CancelOrder")]
        public async Task<ActionResult<OrderResponse>> CancelOrder(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {

                var response = await _orderService.CancelOrder(orderId, request);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hủy đơn hàng thất bại: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
        {
            var rs = await _orderService.GetOrderById(id);
            return Ok(rs);
        }

        [HttpGet("Assigning")]
        public async Task<ActionResult<List<OrderResponse>>> GetOrdersByAssigning()
        {
            var orders = await _orderService.GetOrdersByAssigning();
            return Ok(orders);
        }

        [HttpGet("shipper/{shipperId}")]
        public async Task<ActionResult<List<OrderResponse>>> GetOrderByShipperId(int shipperId)
        {
            var rs = await _orderService.GetOrderByShipperId(shipperId);
            return Ok(rs);
        }

        [HttpGet("{orderId}/TotalPrice")]
        public async Task<ActionResult<TotalPriceResponse>> GetTotalPriceByOrderId(int orderId)
        {
            var rs = await _orderService.GetTotalPriceByOrderId(orderId);
            return Ok(rs);
        }

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
    }
}
