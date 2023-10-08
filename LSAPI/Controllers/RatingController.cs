using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/ratings")]
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;
        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }


        /*[HttpGet("{shipperId}")]
        public async Task<ActionResult<RatingResponse>> GetRatingByShipperId(int shipperId)
        {
            try
            {

                var response = await _ratingService.GetRatingByShipperId(shipperId);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"xem rating thất bại: {ex.Message}");
            }
        
        
        }


        [HttpGet("{shipperId}/AverageRating")]
        public async Task<ActionResult<RatingResponse>> GetAverageRatingByShipperId(int shipperId)
        {
            try
            {

                var response = await _ratingService.GetAverageRatingByShipperId(shipperId);


                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"xem rating thất bại: {ex.Message}");
            }
        }*/

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<RatingResponse>>> GetRating(int id, int shipperId, int ratingValue, int byStoreId, int? pageNumber, int? pageSize)
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
                var rs = await _ratingService.GetRating(id, shipperId, ratingValue, byStoreId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy đánh giá");
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/ratings/count")]
        public async Task<ActionResult<RatingResponse>> GetCountRating()
        {
            try
            {

                var rs = await _ratingService.GetTotalRatingCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPost("register-rating")]
        public async Task<ActionResult<RatingResponse>> CreateRating([FromBody] RegisterRatingRequest request)
        {
            try
            {
                if (request.ShipperId <= 0)
                {
                    return BadRequest("ShipperId phải là số nguyên dương");
                }
                if (request.RatingValue <= 0 || request.RatingValue < 1 && request.RatingValue > 5)
                {
                    return BadRequest("RatingValue chỉ từ 1 đến 5");
                }
                if (request.ByStoreId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                var rs = await _ratingService.CreateRating(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<RatingResponse>> UpdateRating(int id, [FromBody] PutRatingRequest ratingRequest)
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
                if (ratingRequest.ShipperId <= 0)
                {
                    return BadRequest("ShipperId phải là số nguyên dương");
                }
                if (ratingRequest.RatingValue <= 0 || ratingRequest.RatingValue < 1 && ratingRequest.RatingValue > 5)
                {
                    return BadRequest("RatingValue chỉ từ 1 đến 5");
                }
                if (ratingRequest.ByStoreId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                var response = await _ratingService.UpdateRating(id, ratingRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đánh giá thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteRating(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _ratingService.DeleteRating(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đánh giá thất bại: {ex.Message}");
            }
        }
    }
}

