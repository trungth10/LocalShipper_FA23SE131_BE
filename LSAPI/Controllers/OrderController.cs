using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using AutoMapper.Configuration.Conventions;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpGet("ShipperId")]
        public async Task<ActionResult<List<OrderResponse>>> GetOrdersByShipperId(int shipperId)
        {
            var orders = await _orderService.GetOrdersByShipperId(shipperId);
            return Ok(orders);
        }
    }
}
