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
    [Route("api/package-actions")]
    public class PackageActionController : Controller
    {
        private readonly IPackageActionService _packageActionService;
        public PackageActionController(IPackageActionService packageActionService)
        {

            _packageActionService = packageActionService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<PackageActionResponse>>> GetPackageAction(int id, string actionType)
        {
            try
            {
                var rs = await _packageActionService.GetPackageAction(id, actionType);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem PackageAction thất bại: {ex.Message}");
            }

        }
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
        [HttpPut()]
        public async Task<ActionResult<PackageActionResponse>> PutPackageAction(int id, PackageActionRequest packageActionRequest)
        {
            try
            {
                var rs = await _packageActionService.UpdatePackageAction(id, packageActionRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Update PackageAction thất bại: {ex.Message}");
            }
            
        }

        [HttpDelete()]
        public async Task<ActionResult<PackageActionResponse>> DeletePackageAction(int id)
        {
            try
            {
                var rs = await _packageActionService.DeletePackageAction(id);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"xóa PackageAction thất bại: {ex.Message}");
            }
           
        }


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
