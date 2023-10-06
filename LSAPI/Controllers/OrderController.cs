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
using Org.BouncyCastle.Asn1.Ocsp;

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
        public async Task<ActionResult<OrderResponse>> GetOrder(int id, int status, int storeId, int batchId, int? shipperId,
            string tracking_number, string cancel_reason, decimal distance_price,
            decimal subtotal_price, decimal totalPrice, string other, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 0)
                {
                    return BadRequest("Số trang phải là số nguyên dương");
                }

                if (pageSize.HasValue && pageSize <= 0)
                {
                    return BadRequest("Số phần tử trong trang phải là số nguyên dương");
                }
                if (id < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }

                var response = await _orderService.GetOrder(id, status, storeId, batchId, shipperId, 
                    tracking_number, cancel_reason, distance_price, subtotal_price, totalPrice, other, pageNumber, pageSize);


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
                if (request.storeId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                if (request.batchId <= 0)
                {
                    return BadRequest("BatchId phải là số nguyên dương");
                }
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
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (orderRequest.storeId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                if (orderRequest.batchId <= 0)
                {
                    return BadRequest("BatchId phải là số nguyên dương");
                }
                if (orderRequest.status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (orderRequest.distancePrice <= 0)
                {
                    return BadRequest("Số tiền phải là số dương");
                }
                if (orderRequest.subTotalprice <= 0)
                {
                    return BadRequest("Số tiền phải là số dương");
                }
                if (orderRequest.totalPrice <= 0)
                {
                    return BadRequest("Số tiền phải là số dương");
                }

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
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _orderService.DeleteOrder(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đơn hàng thất bại: {ex.Message}");
            }

        }


        //SHIPPER

        [HttpPut("shipper/api/orders")]
        public async Task<ActionResult<MessageResponse>> ShipperToStatusOrder(int id, int shipperId, string cancelReason, OrderStatusEnum status)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _orderService.ShipperToStatusOrder(id, shipperId, cancelReason, status);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đơn hàng thất bại: {ex.Message}");
            }
        }

        [HttpGet("shipper/api/orders/statistical")]
        public async Task<ActionResult<TotalPriceResponse>> GetTotalPriceSumByShipperId(int shipperId, int? month, int? year, int? day)
        {
            try
            {
                if (!year.HasValue || !month.HasValue)
                {
                    return BadRequest("Year and month are required.");
                }
                if (shipperId <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _orderService.GetTotalPriceAndOrderCount(shipperId, month, year, day);
                return Ok(rs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
      
         [HttpGet("shipper/api/orders/statistical-price")]
         public async Task<ActionResult<TotalPriceResponse>> GetTotalPriceSumByShipperId(int shipperId)
         {
            if (shipperId <= 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetTotalPriceSumByShipperId(shipperId);
             return Ok(rs);
         }

         [HttpGet("shipper/api/orders/rate-cancel")]
         public async Task<ActionResult<TotalPriceResponse>> GetCancelRateByShipperId(int shipperId)
         {
            if (shipperId <= 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetCancelRateByShipperId(shipperId);
             return Ok(rs);
         }

         [HttpGet("shipper/api/orders/rate-complete")]
         public async Task<ActionResult<TotalPriceResponse>> GetReceiveRateByShipperId(int shipperId)
         {
            if (shipperId <= 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetReceiveRateByShipperId(shipperId);
             return Ok(rs);
         }         
    }
}
