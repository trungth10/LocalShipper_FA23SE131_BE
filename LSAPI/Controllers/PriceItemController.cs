using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LSAPI.Controllers
{
    [Route("api/price-items")]
    [ApiController]
    public class PriceItemController : ControllerBase
    {
        private readonly IPriceItemService _priceItemService;
        public PriceItemController(IPriceItemService priceItemService)
        {
            _priceItemService = priceItemService;
        }
        [HttpGet()]
        public async Task<ActionResult<List<PriceItemResponse>>> GetPriceItem(int? id, decimal? minAmount,
            decimal? maxAmount, decimal? price, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 0)
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
                var rs = await _priceItemService.GetPriceItem(id, minAmount, maxAmount, price, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giá item");
            }
        }

        [HttpGet("api/price-items/count")]
        public async Task<ActionResult<PriceItemResponse>> GetCountPriceItem()
        {
            try
            {

                var rs = await _priceItemService.GetTotalPriceItemCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-price-item")]
        public async Task<ActionResult<PriceItemResponse>> CreatePriceItem([FromBody] RegisterPriceItemRequest request)
        {
            try
            {
                if (request.MinDistance < 0)
                {
                    return BadRequest("MinDistance phải >= 0");
                }
                if (request.MaxDistance <= 0 || request.MaxDistance <= request.MinDistance)
                {
                    return BadRequest("MaxDistance phải lớn hơn MinDistance");
                }
                if (request.MinAmount <= 0)
                {
                    return BadRequest("MinAmount phải > 0");
                }
                if (request.MaxAmount <= 0 || request.MaxAmount <= request.MinAmount)
                {
                    return BadRequest("MaxAmount phải lớn hơn MinAmount");
                }
                if (request.Price <= 0)
                {
                    return BadRequest("Price phải > 0");
                }
                if (request.PriceId <= 0)
                {
                    return BadRequest("PriceId phải là số nguyên dương");
                }
                var rs = await _priceItemService.CreatePriceItem(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<PriceItemResponse>> UpdatePriceItem(int id, [FromBody] PutPriceItemRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (request.MinDistance < 0)
                {
                    return BadRequest("MinDistance phải >= 0");
                }
                if (request.MaxDistance <= 0 || request.MaxDistance <= request.MinDistance)
                {
                    return BadRequest("MaxDistance phải lớn hơn MinDistance");
                }
                if (request.MinAmount <= 0)
                {
                    return BadRequest("MinAmount phải > 0");
                }
                if (request.MaxAmount <= 0 || request.MaxAmount <= request.MinAmount)
                {
                    return BadRequest("MaxAmount phải lớn hơn MinAmount");
                }
                if (request.Price <= 0)
                {
                    return BadRequest("Price phải > 0");
                }
                if (request.PriceId <= 0)
                {
                    return BadRequest("PriceId phải là số nguyên dương");
                }
                var response = await _priceItemService.UpdatePriceItem(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật giá item thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeletePriceItem(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _priceItemService.DeletePriceItem(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa giá item thất bại: {ex.Message}");
            }
        }
    }
}

