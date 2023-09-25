using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
