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

    
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/orders")]
        public async Task<ActionResult<OrderResponse>> GetOrder(int id, int status, int storeId, int shipperId,
                                     string tracking_number, string cancel_reason, decimal distance_price,
                                     decimal subtotal_price, decimal COD, decimal totalPrice, string other, int routeId,
                                     int capacity, int package_weight, int package_width, int package_height, int package_length,
                                     string customer_city, string customer_commune, string customer_district, string customer_phone,
                                     string customer_name, string customer_email, int actionId, int typeId, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber <= 0)
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

                var response = await _orderService.GetOrder(id, status, storeId, shipperId,
                    tracking_number, cancel_reason, distance_price, subtotal_price, COD, totalPrice, other,
                    routeId, capacity, package_weight, package_width, package_height, package_length,
                    customer_city, customer_commune, customer_district, customer_phone, customer_name,
                    customer_email, actionId, typeId, pageNumber, pageSize);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem đơn hàng thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
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

       

        

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpDelete("api/orders")]
        public async Task<ActionResult<MessageResponse>> DeleteOrder(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (id < 0)
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
        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPut("shipper/api/orders")]
        public async Task<ActionResult<MessageResponse>> ShipperToStatusOrder(int id, int shipperId, string cancelReason, OrderStatusEnum status)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (id < 0)
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

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
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

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("shipper/api/orders/statistical-price")]
        public async Task<ActionResult<TotalPriceResponse>> GetTotalPriceSumByShipperId(int shipperId)
         {
            if (shipperId == 0)
            {
                return BadRequest("Vui lòng nhập ShipperId");
            }
            if (shipperId < 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetTotalPriceSumByShipperId(shipperId);
             return Ok(rs);
         }

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("shipper/api/orders/rate-cancel")]
        public async Task<ActionResult<TotalPriceResponse>> GetCancelRateByShipperId(int shipperId)
         {
            if (shipperId == 0)
            {
                return BadRequest("Vui lòng nhập ShipperId");
            }
            if (shipperId < 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetCancelRateByShipperId(shipperId);
             return Ok(rs);
         }


        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("shipper/api/orders/rate-complete")]
        public async Task<ActionResult<TotalPriceResponse>> GetReceiveRateByShipperId(int shipperId)
         {
            if (shipperId == 0)
            {
                return BadRequest("Vui lòng nhập ShipperId");
            }
            if (shipperId < 0)
            {
                return BadRequest("Id phải là số nguyên dương");
            }
            var rs = await _orderService.GetReceiveRateByShipperId(shipperId);
             return Ok(rs);
         }         
    }
}
