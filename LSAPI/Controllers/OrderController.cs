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
using LocalShipper.Data.Models;
using MailKit.Search;
using System.Text.RegularExpressions;

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
        public async Task<ActionResult<OrderResponse>> GetOrder(int zoneId, int id, int status, int storeId, int shipperId,
                                     string tracking_number, string cancel_reason, decimal distance, decimal distance_price,
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

                var response = await _orderService.GetOrder(zoneId,id, status, storeId, shipperId,
                    tracking_number, cancel_reason, distance, distance_price, subtotal_price, COD, totalPrice, other,
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
        [HttpPost("api/orders/v2")]
        public async Task<ActionResult<OrderResponse>> GetOrderV2( [FromBody] OrderRequestV2 request, int? pageNumber, int? pageSize)
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
               

                var response = await _orderService.GetOrderV2(request, pageNumber, pageSize);


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


        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPost("api/orders/create")]
        public async Task<ActionResult<OrderCreateResponse>> CreateOrder(OrderRequestForCreate request)
        {
            try
            {
                if (request.StoreId < 1)
                {
                    return BadRequest("storeId phải là số dương");
                }              
                if (request.SubtotalPrice < 0)
                {
                    return BadRequest("SubtotalPrice phải là số dương");
                }
                if (request.Cod < 0)
                {
                    return BadRequest("Cod phải là số dương");
                }
                if (request.PackageWidth < 0)
                {
                    return BadRequest("PackageWidth phải là số dương");
                }
                if (request.PackageHeight < 0)
                {
                    return BadRequest("PackageHeight phải là số dương");
                }
                if (request.PackageLength < 0)
                {
                    return BadRequest("PackageLength phải là số dương");
                }
                if (request.PackageWeight < 0)
                {
                    return BadRequest("PackageWeight phải là số dương");
                }
                if (request.TypeId < 1)
                {
                    return BadRequest("TypeId phải là số dương");
                }
                if (request.ActionId < 1)
                {
                    return BadRequest("TypeId phải là số dương");
                }
                var regex2 = new Regex("^[0-9]+$");

                if (!regex2.IsMatch(request.CustomerPhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (request.CustomerPhone.Length < 9 || request.CustomerPhone.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }
                var regex = new Regex(@"^\w+@gmail\.com$");
                if (!regex.IsMatch(request.CustomerEmail))
                {
                    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                }
                if (string.IsNullOrWhiteSpace(request.CustomerEmail))
                {
                    return BadRequest("CustomerEmail không được để trống");
                }
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return BadRequest("CustomerName không được để trống");
                }
                var regex3 = new Regex(@"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");
                if (regex3.IsMatch(request.CustomerName))
                {
                    return BadRequest("CustomerName không được chứa ký tự đặc biệt");
                }
                if (string.IsNullOrWhiteSpace(request.CustomerDistrict))
                {
                    return BadRequest("CustomerDistrict không được để trống");
                }
                if (string.IsNullOrWhiteSpace(request.CustomerCity))
                {
                    return BadRequest("CustomerCity không được để trống");
                }
                if (string.IsNullOrWhiteSpace(request.CustomerCommune))
                {
                    return BadRequest("CustomerCommune không được để trống");
                }
                var rs = await _orderService.CreateOrder(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"tạo Order thất bại: {ex.Message}");
            }
           
        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPut("api/orders/update")]
        public async Task<ActionResult<OrderResponse>> UpdateOrder(int id, OrderRequestForUpdate request)
        {
            try 
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn nhập Id");
                }
                if (id < 0)
                {
                    return BadRequest("id phải là số dương");
                }
                if (request.DistancePrice < 0)
                {
                    return BadRequest("DistancePrice phải là số dương");
                }
                if (request.SubtotalPrice < 0)
                {
                    return BadRequest("SubtotalPrice phải là số dương");
                }
                if (request.Cod < 0)
                {
                    return BadRequest("Cod phải là số dương");
                }
                if (request.PackageWidth < 0)
                {
                    return BadRequest("PackageWidth phải là số dương");
                }
                if (request.PackageHeight < 0)
                {
                    return BadRequest("PackageHeight phải là số dương");
                }
                if (request.PackageLength < 0)
                {
                    return BadRequest("PackageLength phải là số dương");
                }
                if (request.PackageWeight < 0)
                {
                    return BadRequest("PackageWeight phải là số dương");
                }
                if (request.TypeId < 1)
                {
                    return BadRequest("TypeId phải là số dương");
                }
                if (request.ActionId < 1)
                {
                    return BadRequest("TypeId phải là số dương");
                }
                //var regex2 = new Regex("^[0-9]+$");

                //if (!regex2.IsMatch(request.CustomerPhone))
                //{
                //    return BadRequest("Số điện thoại không hợp lệ");
                //}
                //if (request.CustomerPhone.Length < 9 || request.CustomerPhone.Length > 11)
                //{
                //    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                //}
                //var regex = new Regex(@"^\w+@gmail\.com$");
                //if (!regex.IsMatch(request.CustomerEmail))
                //{
                //    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                //}
                //var regex3 = new Regex(@"[!@#$%^&*()_+{}\[\]:;<>,.?~\\-]");
                //if (regex3.IsMatch(request.CustomerName))
                //{
                //    return BadRequest("CustomerName không được chứa ký tự đặc biệt");
                //}
                var rs = await _orderService.UpdateOrder(id, request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"cập nhật Order thất bại: {ex.Message}");
            }
            
        }

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("shipper/api/orders/suggest")]
        public async Task<ActionResult<OrderResponse>> GetOrderSuggest(int id, SuggestEnum suggest, int money)
        {
            try
            {

                var rs = await _orderService.GetOrderSuggest(id, suggest,money);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Gợi ý hóa đơn thất bại: {ex.Message}");
            }

        }
    }
}
