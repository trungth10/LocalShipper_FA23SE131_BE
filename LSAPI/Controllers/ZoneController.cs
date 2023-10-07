﻿using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/zones")]
    [Authorize(Roles = Roles.Staff + "," + Roles.Admin)]
    public class ZoneController : Controller
    {
        private readonly IZoneService _zoneService;

        public ZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet()]
        [Authorize]
        public async Task<ActionResult<ZoneResponse>> GetZones(int id, string zoneName, decimal latitude, decimal longtitude, decimal radius, int? pageNumber, int? pageSize)
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
                var rs = await _zoneService.GetZones(id, zoneName, latitude, longtitude, radius, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem khu vực thất bại: {ex.Message}");
            }

        }

        [HttpGet("api/zones/count")]
        [Authorize]
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
        [Authorize(Roles = Roles.Admin + "," + Roles.Staff)]
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
        [Authorize(Roles = Roles.Admin + "," + Roles.Staff)]
        public async Task<ActionResult<ZoneResponse>> UpdateZone(int id, [FromBody] ZoneRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _zoneService.UpdateZone(id,request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật khu vực thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        [Authorize(Roles = Roles.Admin + "," + Roles.Staff)]
        public async Task<ActionResult<ZoneResponse>> DeleteZone(int id)
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
