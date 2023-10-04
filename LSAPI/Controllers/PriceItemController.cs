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
                var rs = await _priceItemService.GetPriceItem(id, minAmount, maxAmount, price, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giá item");
            }
        }

        [HttpGet("count")]
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

