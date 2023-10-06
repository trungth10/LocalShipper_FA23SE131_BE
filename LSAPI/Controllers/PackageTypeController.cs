﻿using LocalShipper.Service.DTOs.Request;
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
    [Route("api/package-types")]
    public class PackageTypeController : Controller
    {
        private readonly IPackageTypeService _packageTypeService;
        public PackageTypeController(IPackageTypeService packageTypeService)
        {
            _packageTypeService = packageTypeService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<PackageTypeResponse>>> GetPackageType(int id, string packageType, int? pageNumber, int? pageSize)
        {
            try
            {
                if (id < 0)
                {
                    return BadRequest("id phải là số dương");
                }
                if (pageNumber.HasValue && pageNumber < 0)
                {
                    return BadRequest("pageNumber phải là số dương");
                }

                if (pageSize.HasValue && pageSize < 0)
                {
                    return BadRequest("pageSize phải là số dương");
                }


                var rs = await _packageTypeService.GetPackageType(id, packageType, pageNumber, pageSize);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Xem PackageType thất bại: {ex.Message}");
            }
            
        }
        [HttpPost()]
        public async Task<ActionResult<PackageTypeResponse>> PostPackageType(PackageTypeRequest request)
        {
            try
            {


                var rs = await _packageTypeService.CreatePackageType(request);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"tạo PackageType thất bại: {ex.Message}");
            }
           
        }

        [HttpPut()]
        public async Task<ActionResult<PackageTypeResponse>> PutPackageType(int id, PackageTypeRequest packageTypeRequest)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id <= 0)
                {
                    return BadRequest("id phải là số dương");
                }
                
                var rs = await _packageTypeService.UpdatePackageType(id, packageTypeRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"update PackageType thất bại: {ex.Message}");
            }
          
        }

        [HttpGet("count")]
        public async Task<ActionResult<PackageTypeResponse>> GetCountPackageType()
        {
            try
            {

                var rs = await _packageTypeService.GetTotalPackageTypeCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
