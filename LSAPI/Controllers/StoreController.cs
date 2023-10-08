using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/stores")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<StoreResponse>>> GetStore(int id, string storeName, int status, int zoneId, int walletId, int accountId, int? pageNumber, int? pageSize)
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
                    return BadRequest("Id phải là số nguyên dương");
                }
                
                if (status < 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (zoneId < 0)
                {
                    return BadRequest("zoneId phải là số nguyên dương");
                }
                if (walletId < 0)
                {
                    return BadRequest("walletId phải là số nguyên dương");
                }
                if (accountId < 0)
                {
                    return BadRequest("accountId phải là số nguyên dương");
                }
                var rs = await _storeService.GetStore(id, storeName, status, zoneId, walletId, accountId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem Store thất bại: {ex.Message}");
            }

        }

        [AllowAnonymous]
        [HttpPost()]
        public async Task<ActionResult<StoreResponse>> PostStore(StoreRequest request)
        {
            try
            {

                var regex2 = new Regex("^[0-9]+$");

                if (!regex2.IsMatch(request.StorePhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (request.StorePhone.Length < 9 || request.StorePhone.Length > 11)
                {
                    return BadRequest("Số điện thoại phải có từ 9 đến 11 số");
                }
                if (request.Status <0)
                {
                    return BadRequest("Status phải là số nguyên dương");
                }
                if (request.ZoneId < 0)
                {
                    return BadRequest("zoneId phải là số nguyên dương");
                }
                if (request.WalletId < 0)
                {
                    return BadRequest("walletId phải là số nguyên dương");
                }
                if (request.AccountId < 0)
                {
                    return BadRequest("accountId phải là số nguyên dương");
                }
                if (request.TemplateId < 0)
                {
                    return BadRequest("TemplateId phải là số nguyên dương");
                }
                var regex = new Regex(@"^\w+@gmail\.com$");
                if (!regex.IsMatch(request.StoreEmail))
                {
                    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                }
                //var regexTime = new Regex(@"^\d{1,2}:\d{2}:\d{2}$");
                //if (!regexTime.IsMatch(request.OpenTime.ToString()) || !regexTime.IsMatch(request.CloseTime.ToString()))
                //{
                //    return BadRequest("Giờ mở cửa và đóng cửa phải có định dạng 'HH:mm:ss', ví dụ: '7:00:00'");
                //}
                //if (!TimeSpan.TryParse(request.OpenTime.Value.ToString(), out _))
                //{
                //    ModelState.AddModelError("$.openTime", "Giờ mở cửa không hợp lệ");
                //}
                //if (!TimeSpan.TryParse(request.CloseTime.Value.ToString(), out _))
                //{
                //    ModelState.AddModelError("$.closeTime", "Giờ đóng cửa không hợp lệ");
                //}
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(ModelState);
                //}
                var rs = await _storeService.CreateStore(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"tạo Store thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<StoreResponse>> PutStore(int id, StoreRequest storeRequest)
        {
            try
            {
                var regex2 = new Regex("^[0-9]+$");

                if (!regex2.IsMatch(storeRequest.StorePhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (storeRequest.StorePhone.Length < 9 || storeRequest.StorePhone.Length > 11)
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
                if (storeRequest.Status < 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (storeRequest.ZoneId < 0)
                {
                    return BadRequest("zoneId phải là số nguyên dương");
                }
                if (storeRequest.WalletId < 0)
                {
                    return BadRequest("walletId phải là số nguyên dương");
                }
                if (storeRequest.AccountId < 0)
                {
                    return BadRequest("accountId phải là số nguyên dương");
                }
                if (storeRequest.TemplateId < 0)
                {
                    return BadRequest("TemplateId phải là số nguyên dương");
                }
                var regex = new Regex(@"^\w+@gmail\.com$");
                if (!regex.IsMatch(storeRequest.StoreEmail))
                {
                    return BadRequest("Email phải có địa chỉ tên miền @gmail.com");
                }
                var rs = await _storeService.UpdateStore(id, storeRequest);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"update Store thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
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
                var rs = await _storeService.DeleteStore(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa Store thất bại: {ex.Message}");
            }

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("count")]
        public async Task<ActionResult<StoreResponse>> GetCountStore()
        {
            try
            {

                var rs = await _storeService.GetTotalStoreCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
