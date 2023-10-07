using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.Services.Implement;
using Org.BouncyCastle.Asn1.Ocsp;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LSAPI.Controllers
{
    [Route("api/prices")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IPriceLSService _priceService;
        public PriceController(IPriceLSService priceService)
        {
            _priceService = priceService;
        }


        [HttpGet()]
        public async Task<ActionResult<List<PriceLSResponse>>> GetPriceItem(int? id, string? name, int? storeId, int? hourFilter,
            int? dateFilter, int? mode, int? status, int? priority, int? pageNumber, int? pageSize)
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
                var rs = await _priceService.GetPrice(id, name, storeId, hourFilter, dateFilter, mode, status, priority, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giá");
            }
        }

        [HttpGet("api/prices/count")]
        public async Task<ActionResult<PriceInZoneResponse>> GetCountPrice()
        {
            try
            {

                var rs = await _priceService.GetTotalPriceCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

        [HttpPut()]
        public async Task<ActionResult<PriceLSResponse>> UpdatePrice(int id, [FromBody] PutPriceRequest priceRequest, int accountId)
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
                if (accountId <= 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (priceRequest.Mode <= 0)
                {
                    return BadRequest("Mode phải là số nguyên dương");
                }
                if (priceRequest.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (priceRequest.Priority <= 0)
                {
                    return BadRequest("Priority phải là số nguyên dương");
                }
                var response = await _priceService.UpdatePrice(id, priceRequest, accountId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật bảng giá thất bại: {ex.Message}");
            }
        }



        [HttpPost("register-price")]
        public async Task<ActionResult<PriceLSResponse>> CreatePrice([FromBody] RegisterPriceRequest request, int accountId)
        {
            try
            {
                if (accountId == 0)
                {
                    return BadRequest("Vui lòng nhập AccountId");
                }
                if (accountId < 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (request.Mode <= 0)
                {
                    return BadRequest("Mode phải là số nguyên dương");
                }
                if (request.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (request.Priority <= 0)
                {
                    return BadRequest("Priority phải là số nguyên dương");
                }
                var rs = await _priceService.CreatePrice(request, accountId);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeletePrice(int id)
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
                var response = await _priceService.DeletePrice(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa giá thất bại: {ex.Message}");
            }
        }
    }
}
