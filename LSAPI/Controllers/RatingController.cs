using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
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
       

        [HttpGet("{shipperId}")]
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
        }


        [HttpGet()]
        public async Task<ActionResult<List<RatingResponse>>> GetRating(int id, int shipperId, int ratingValue, int byStoreId)
        {
            try
            {
                var rs = await _ratingService.GetRating(id, shipperId, ratingValue, byStoreId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy đánh giá");
            }
        }


        [HttpGet("count.json")]
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


        [HttpPost("register-rating")]
        public async Task<ActionResult<RatingResponse>> CreateRating([FromBody] RegisterRatingRequest request)
        {
            try
            {
                var rs = await _ratingService.CreateRating(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<RatingResponse>> UpdateRating(int id, [FromBody] PutRatingRequest ratingRequest)
        {
            try
            {

                var response = await _ratingService.UpdateRating(id, ratingRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đánh giá thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteRating(int id)
        {
            try
            {

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

