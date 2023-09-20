using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.Services.Interface;
using LocalShipper.Service.Services.Implement;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShipperController : Controller
    {

        private readonly IShipperService _shipperService;

        public ShipperController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        [HttpPut("{shipperId:int}/status")]
        public async Task<ActionResult<ShipperResponse>> UpdateShipperStatus(int shipperId, [FromBody] UpdateShipperStatusRequest request)
        {
            try
            {
               
               // int agentId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
                var response = await _shipperService.UpdateShipperStatus(shipperId, request);

                
                return Ok(response);
            }
            catch (Exception ex)
            {              
                return BadRequest($"Cập nhật trạng thái người giao hàng thất bại: {ex.Message}");
            }
        }
    }
}
