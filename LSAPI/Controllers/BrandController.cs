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
    [Route("api/[controller]")]
    [ApiController]
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
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<BrandResponse>>> GetBrands([FromQuery] BrandPagingRequest request)
        {
            var rs = await _brandService.GetBrands(request);
            return Ok(rs);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<BrandResponse>> GetBrandById(int id)
        {
            var rs = await _brandService.GetBrandByID(id);
            return Ok(rs);
        }
        

        
        /// <summary>
        /// Delete Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Roles.Brand + "," + Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BrandResponse>> DeleteBrand(int id)
        {
            var rs = await _brandService.DeleteBrand(id);
            return Ok(rs);
        }


        
    }

}
