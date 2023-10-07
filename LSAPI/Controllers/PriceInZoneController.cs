using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [Route("api/price-in-zones")]
    [ApiController]
    public class PriceInZoneController : ControllerBase
    {
        private readonly IPriceInZoneService _priceInZoneService;
        public PriceInZoneController(IPriceInZoneService priceInZoneService)
        {
            _priceInZoneService = priceInZoneService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<PriceInZoneResponse>>> GetPriceInZone(int? id, int? priceId,
            int? zoneId, int? pageNumber, int? pageSize)
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
                var rs = await _priceInZoneService.GetPriceInZone(id, priceId, zoneId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy bảng giá trong khu vực");
            }
        }

        [HttpGet("api/price-in-zones/count")]
        public async Task<ActionResult<PriceInZoneResponse>> GetCountPriceInZone()
        {
            try
            {

                var rs = await _priceInZoneService.GetTotalPriceInZoneCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-price-in-zone")]
        public async Task<ActionResult<PriceInZoneResponse>> CreatePriceInZone([FromBody] RegisterPriceInZoneRequest request)
        {
            try
            {
                if (request.priceId <= 0)
                {
                    return BadRequest("PriceId phải là số nguyên dương");
                }
                if (request.zoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                var rs = await _priceInZoneService.CreatePriceInZone(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<PriceInZoneResponse>> UpdatePriceInZone(int id, [FromBody] PutPriceInZoneRequest priceInZonerequest)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (priceInZonerequest.priceId <= 0)
                {
                    return BadRequest("PriceId phải là số nguyên dương");
                }
                if (priceInZonerequest.zoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                var response = await _priceInZoneService.UpdatePriceInZone(id, priceInZonerequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật bảng giá thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeletePriceItem(int id)
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
                var response = await _priceInZoneService.DeletePriceInZone(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa bảng giá thất bại: {ex.Message}");
            }
        }
    }
}

