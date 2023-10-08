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
    [Route("api/package-actions")]
    public class PackageActionController : Controller
    {
        private readonly IPackageActionService _packageActionService;
        public PackageActionController(IPackageActionService packageActionService)
        {

            _packageActionService = packageActionService;
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<PackageActionResponse>>> GetPackageAction(int id, string actionType, int? pageNumber, int? pageSize)
        {
            try
            {
                if(id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (pageNumber.HasValue && pageNumber <= 0)
                {
                    return BadRequest("Số trang phải là số nguyên dương");
                }

                if (pageSize.HasValue && pageSize <= 0)
                {
                    return BadRequest("Số phần tử trong trang phải là số nguyên dương");
                }
                var rs = await _packageActionService.GetPackageAction(id, actionType, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem PackageAction thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpPost()]
        public async Task<ActionResult<PackageActionResponse>> PostPackageAction(PackageActionRequest request)
        {
            try
            { 
                var rs = await _packageActionService.CreatePackageAction(request);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"tạo PackageAction thất bại: {ex.Message}");
            }
           
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<PackageActionResponse>> PutPackageAction(int id, PackageActionRequest packageActionRequest)
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
                var rs = await _packageActionService.UpdatePackageAction(id, packageActionRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Update PackageAction thất bại: {ex.Message}");
            }
            
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<PackageActionResponse>> DeletePackageAction(int id)
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
                var rs = await _packageActionService.DeletePackageAction(id);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"xóa PackageAction thất bại: {ex.Message}");
            }
           
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpGet("count")]
        public async Task<ActionResult<PackageActionResponse>> GetCountPackageAction()
        {
            try
            {

                var rs = await _packageActionService.GetTotalPackageActionCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
