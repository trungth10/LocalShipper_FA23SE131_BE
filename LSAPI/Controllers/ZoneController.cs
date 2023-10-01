using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/zones")]
    public class ZoneController : Controller
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet("")]
        public async Task<ActionResult<ZoneResponse>> GetZones(int id, string zoneName, decimal latitude, decimal longtitude, decimal radius)
        {
            try
            {
                var rs = await _zoneService.GetZones(id, zoneName, latitude, longtitude, radius);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem khu vực thất bại: {ex.Message}");
            }

        }

        [HttpGet("count")]
        public async Task<ActionResult<ZoneResponse>> GetCount()
        {
            try
            {
                var rs = await _zoneService.GetTotalZoneCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count khu vực thất bại: {ex.Message}");
            }

        }

        [HttpPost()]
        public async Task<ActionResult<ZoneResponse>> Createzone([FromBody] ZoneRequest request)
        {
            try
            {
                var rs = await _zoneService.CreateZone(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Tạo mới khu vực thất bại: {ex.Message}");
            }

        }

        [HttpPut()]
        public async Task<ActionResult<ZoneResponse>> UpdateZone(int id, [FromBody] ZoneRequest request)
        {
            try
            {
                var rs = await _zoneService.UpdateZone(id,request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật khu vực thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<ZoneResponse>> DeleteZone(int id)
        {
            try
            {
                var rs = await _zoneService.DeleteZone(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa khu vực thất bại: {ex.Message}");
            }
        }


    }
}
