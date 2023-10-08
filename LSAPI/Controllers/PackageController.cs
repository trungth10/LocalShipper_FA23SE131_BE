using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackageController : Controller
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<PackageResponse>>> GetPackage(int batchId, int id,
            int status, int actionId, int typeId, int storeId, string customerName,
             int? pageNumber, int? pageSize)
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
                if (batchId < 0)
                {
                    return BadRequest("batchId phải là số nguyên dương");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (status < 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (actionId < 0)
                {
                    return BadRequest("actionId phải là số nguyên dương");
                }
                if (typeId < 0)
                {
                    return BadRequest("typeOd phải là số nguyên dương");
                }
                if (storeId < 0)
                {
                    return BadRequest("storeId phải là số nguyên dương");
                }
                var package = await _packageService.GetPackage(batchId, id, status, actionId, typeId, storeId,
                    customerName, pageNumber, pageSize);
                return Ok(package);

            }
            catch (Exception ex)
            {
                return BadRequest($"Xem Package thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpPost()]
        public async Task<ActionResult<PackageResponse>> PostPackage(PackageRequestForCreate request)
        {
            try
            {
                var regex2 = new Regex("^[0-9]+$");

                if (!regex2.IsMatch(request.CustomerPhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (request.CustomerPhone.Length < 9 || request.CustomerPhone.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }
                if (request.StoreId < 0)
                {
                    return BadRequest("storeId phải là số nguyên dương");
                }
                if (request.Capacity < 0)
                {
                    return BadRequest("Capacity phải là số dương");
                }
                if (request.PackageWidth < 0)
                {
                    return BadRequest("PackageWidth phải là số dương");
                }
                if (request.PackageWeight < 0)
                {
                    return BadRequest("PackageWeight phải là số dương");
                }
                if (request.PackageHeight < 0)
                {
                    return BadRequest("PackageHeight phải là số dương");
                }
                if (request.PackageLength < 0)
                {
                    return BadRequest("PackageLength phải là số dương");
                }
                var regex = new Regex(@"^\w+@gmail\.com$");
                if (!regex.IsMatch(request.CustomerEmail))
                {
                    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                }
                if (request.PackagePrice < 0)
                {
                    return BadRequest("PackagePrice phải là số dương");
                }
                if (request.DistancePrice < 0)
                {
                    return BadRequest("DistancePrice phải là số dương");
                }
                if (request.SubtotalPrice < 0)
                {
                    return BadRequest("SubtotalPrice phải là số dương");
                }
                if (request.ActionId < 0)
                {
                    return BadRequest("ActionId phải là số nguyên dương");
                }
                if (request.TypeId < 0)
                {
                    return BadRequest("TypeId phải là số nguyên dương");
                }
                var rs = await _packageService.CreatePackage(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"tạo Package thất bại: {ex.Message}");
            }

        }


        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<PackageResponse>> PutPackage(int id, PackageRequestForCreate request)
        {
            try
            {
                var regex2 = new Regex("^[0-9]+$");

                if (!regex2.IsMatch(request.CustomerPhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (request.CustomerPhone.Length < 9 || request.CustomerPhone.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }

                if (id == 0)
                {
                    return BadRequest("Vui lòng nhập Id");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }

                if (request.Capacity <= 0)
                {
                    return BadRequest("Capacity phải là số dương");
                }
                if (request.PackageWidth <= 0)
                {
                    return BadRequest("PackageWidth phải là số dương");
                }
                if (request.PackageWeight <= 0)
                {
                    return BadRequest("PackageWeight phải là số dương");
                }
                if (request.PackageHeight <= 0)
                {
                    return BadRequest("PackageHeight phải là số dương");
                }
                if (request.PackageLength <= 0)
                {
                    return BadRequest("PackageLength phải là số dương");
                }
                var regex = new Regex(@"^\w+@gmail\.com$");
                if (!regex.IsMatch(request.CustomerEmail))
                {
                    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                }
                if (request.PackagePrice < 0)
                {
                    return BadRequest("PackagePrice phải là số dương");
                }
                if (request.DistancePrice < 0)
                {
                    return BadRequest("DistancePrice phải là số dương");
                }
                if (request.SubtotalPrice < 0)
                {
                    return BadRequest("SubtotalPrice phải là số dương");
                }
                if (request.ActionId <= 0)
                {
                    return BadRequest("ActionId phải là số nguyên dương");
                }
                if (request.TypeId <=0)
                {
                    return BadRequest("TypeId phải là số nguyên dương");
                }


                var rs = await _packageService.UpdatePackage(id, request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"update Package thất bại: {ex.Message}");
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("status")]
        public async Task<ActionResult<PackageResponse>> PutStatusPackage(int id, PackageStatusEnum status)
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
                if(status <= 0)
                {
                    return BadRequest("status phải là số dương");
                }
                var rs = await _packageService.UpdateStatusPackage(id, status);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"update status Package thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<PackageResponse>> DeletePackage(int id)
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
                var rs = await _packageService.DeletePackage(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"xóa Package thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("count")]
        public async Task<ActionResult<PackageResponse>> GetCountPackage(int batchId)
        {
            try
            {

                var rs = await _packageService.GetTotalPackageCount(batchId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
