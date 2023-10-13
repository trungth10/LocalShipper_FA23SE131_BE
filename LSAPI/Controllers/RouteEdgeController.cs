using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

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


    }
}
