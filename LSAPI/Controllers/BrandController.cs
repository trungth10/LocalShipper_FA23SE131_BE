using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Get Brands
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        
        [HttpGet]
        public async Task<ActionResult<List<BrandResponse>>> GetBrands(int id, string brandName, string brandDescripton, string iconUrl, string imageUrl, int accountId)
        {
            var rs = await _brandService.GetBrands(id, brandName, brandDescripton, iconUrl, imageUrl, accountId);
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<BrandResponse>> PostBrand(BrandRequest request)
        {
            var rs = await _brandService.PostBrand(request);
            return Ok(rs);
        }

        [HttpPut()]
        public async Task<ActionResult<BrandResponse>> PutBrand(int id, BrandRequest brandRequest)
        {
            var rs = await _brandService.UpdateBrand(id, brandRequest);
            return Ok(rs);
        }


        /// <summary>
        /// Delete Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
      
        [HttpDelete("{id}")]
        public async Task<ActionResult<BrandResponse>> DeleteBrand(int id)
        {
            var rs = await _brandService.DeleteBrand(id);
            return Ok(rs);
        }


        
    }

}
