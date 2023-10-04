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
    [Route("api/packages")]
    //[Authorize(Roles = Roles.Shipper + "," + Roles.Store)]
    public class PackageController : Controller
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet()]

        public async Task<ActionResult<List<PackageResponse>>> GetPackage(int batchId, int id, int status, int actionId, int typeId,int storeId, string customerName, string customerAddress, string customerPhome, string custommerEmail, decimal totalPrice)
        {
            try
            {
                var package = await _packageService.GetPackage(batchId, id, status, actionId, typeId, storeId, customerName, customerAddress, customerPhome, custommerEmail, totalPrice);
                return Ok(package);

            }
            catch (Exception ex)
            {
                return BadRequest($"Xem Package thất bại: {ex.Message}");
            }

        }
        [HttpPost()]
        public async Task<ActionResult<PackageResponse>> PostPackage(PackageRequestForCreate request)
        {
            try
            {
                var rs = await _packageService.CreatePackage(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"tạo Package thất bại: {ex.Message}");
            }

        }
        [HttpPut()]
        public async Task<ActionResult<PackageResponse>> PutPackage(int id, PackageRequest packageRequest)
        {
            try
            {
                var rs = await _packageService.UpdatePackage(id, packageRequest);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"update Package thất bại: {ex.Message}");
            }

        }

        [HttpPut("status")]
        public async Task<ActionResult<PackageResponse>> PutStatusPackage(int id,  PackageStatusEnum status)
        {
            try
            {
                var rs = await _packageService.UpdateStatusPackage(id, status);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"update status Package thất bại: {ex.Message}");
            }

        }

        [HttpDelete()]
        public async Task<ActionResult<PackageResponse>> DeletePackage(int id)
        {
            try
            {
                var rs = await _packageService.DeletePackage(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"xóa Package thất bại: {ex.Message}");
            }

        }

        [HttpGet("count")]
        public async Task<ActionResult<PackageResponse>> GetCountPackage()
        {
            try
            {

                var rs = await _packageService.GetTotalPackageCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
