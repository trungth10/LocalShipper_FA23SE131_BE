using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [ApiController]
    public class RouteEdgeController : Controller
    {
         

        private readonly IRouteService _routeService;
        public RouteEdgeController(IRouteService routeService)
        {
            _routeService = routeService;
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
        public async Task<ActionResult<RouteEdgeResponse>> AddOrderToRoute(IEnumerable<int> id, int shipperId)
        {
            try
            {               
                if (shipperId < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }

                var response = await _routeService.AddOrderToRoute(id, shipperId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm vận đơn vào lộ trình thất bại: {ex.Message}");
            }
        }


    }
}
