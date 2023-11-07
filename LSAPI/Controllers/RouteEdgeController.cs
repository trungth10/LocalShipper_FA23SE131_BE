using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using LocalShipper.Service.DTOs.Request;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LSAPI.Controllers
{
    [ApiController]
    public class RouteEdgeController : Controller
    {


        private readonly IRouteService _routeService;
        
        public RouteEdgeController(IRouteService routeService )
        {
            _routeService = routeService;
           
        }

        [HttpGet("api/routes/convert")]
        public async Task<ActionResult<GeocodingResponse>> ConvertAddress(string address)
        {
            try
            {

                var response = await _routeService.ConvertAddress(address);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Chuyển địa chỉ thất bại: {ex.Message}");
            }
        }




        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("api/routes")]
        public async Task<ActionResult<RouteEdgeResponse>> GetRoute(int id, string fromStation, string toStation, int quantity, int progress, int priority, int status, int shipperId, int? pageNumber, int? pageSize)
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

                var response = await _routeService.GetRoute(id, fromStation, toStation, quantity, progress, priority, status, shipperId, pageNumber, pageSize);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem lộ trình thất bại: {ex.Message}");
            }
        }


        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPost("shipper/api/routes")]
        public async Task<ActionResult<RouteEdgeResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId, int routeId)
        {
            try
            {
                if (shipperId < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }

                var response = await _routeService.AddOrderToRoute(id, shipperId, routeId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm vận đơn vào lộ trình thất bại: {ex.Message}");
            }
        }

        //[Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPost("shipper/api/routes-auto")]
        public async Task<ActionResult<OrderResponse>> CreateRouteSuggest(int shiperId, int money, SuggestEnum suggest, int capacityLow, int capacityHight, CreateRouteRequestAuto request)
        {
            try
            {
                if (shiperId < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }

                var response = await _routeService.CreateRouteSuggest(shiperId, money, suggest, capacityLow, capacityHight, request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm tự động vận đơn vào lộ trình thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPost("api/routes")]
        public async Task<ActionResult<RouteEdgeResponse>> AddRoute(CreateRouteRequest request)
        {
            try
            {


                var response = await _routeService.CreateRoute(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm vận đơn vào lộ trình thất bại: {ex.Message}");
            }
        }
        [Authorize(Roles = Roles.Shipper + "," + Roles.Staff + "," + Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPut("api/routes")]
        public async Task<ActionResult<RouteEdgeResponse>> UpdateRoute(int routeId, RouteRequest request)
        {
            try
            {
                if (request.Quantity < 1)
                {
                    return BadRequest("Quantity phải là số dương");
                }
                if (request.Progress < 1)
                {
                    return BadRequest("Progress phải là số dương");
                }
                if (request.Priority < 1)
                {
                    return BadRequest("Priority phải là số dương");
                }
                if (request.Status < 1)
                {
                    return BadRequest("Status phải là số dương");
                }
                if (request.ShipperId < 1)
                {
                    return BadRequest("ShipperId phải là số dương");
                }

                if (routeId < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }
                if (routeId < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }

                var response = await _routeService.UpdateRoute(routeId, request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"cập nhật lộ trình thất bại: {ex.Message}");
            }
        }
        [Authorize(Roles = Roles.Shipper + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpDelete("api/routes")]
        public async Task<ActionResult<MessageResponse>> DeleteOrder(int routeId)
        {
            try
            {
                if (routeId == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (routeId < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _routeService.DeleteRoute(routeId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đơn hàng thất bại: {ex.Message}");
            }

        }


        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpDelete("api/routes/routeid")]
        public async Task<ActionResult<MessageResponse>> DeleteOrderidInRoute(IEnumerable<int> orderid)
        {
            try
            {
                var response = await _routeService.UpdateOrderRouteId(orderid);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đơn hàng thất bại: {ex.Message}");

            }

        }
    }
}
