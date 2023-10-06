using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
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

        [HttpGet()]
        public async Task<ActionResult<List<StoreResponse>>> GetStore(int id, string storeName, int status, int zoneId, int walletId, int accountId, int? pageNumber, int? pageSize)
        {
            try
            {
                var rs = await _storeService.GetStore(id, storeName, status, zoneId, walletId, accountId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Xem Store thất bại: {ex.Message}");
            }
           
        }
        [HttpPost()]
        public async Task<ActionResult<StoreResponse>> PostStore(StoreRequest request)
        {
            try
            {
                var regex = new Regex("^[0-9]+$");
                var regex2 = new Regex("^[a-zA-Z]+$");
                if (!regex.IsMatch(request.StoreName))
                {
                    return BadRequest("Tên cửa hàng không hợp lệ");
                }
                if (!regex2.IsMatch(request.StorePhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (request.TemplateId <= 0)
                {
                    return BadRequest("TemplateId phải là số nguyên dương");
                }
                if (request.AccountId <= 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (request.ZoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                if (request.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (request.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                var rs = await _storeService.CreateStore(request);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Tạo Store thất bại: {ex.Message}");
            }
            
        }
        [HttpPut()]
        public async Task<ActionResult<StoreResponse>> PutStore(int id, StoreRequest storeRequest)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var regex = new Regex("^[0-9]+$");
                var regex2 = new Regex("^[a-zA-Z]+$");
                if (!regex.IsMatch(storeRequest.StoreName))
                {
                    return BadRequest("Tên cửa hàng không hợp lệ");
                }
                if (!regex2.IsMatch(storeRequest.StorePhone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                if (storeRequest.TemplateId <= 0)
                {
                    return BadRequest("TemplateId phải là số nguyên dương");
                }
                if (storeRequest.AccountId <= 0)
                {
                    return BadRequest("AccountId phải là số nguyên dương");
                }
                if (storeRequest.ZoneId <= 0)
                {
                    return BadRequest("ZoneId phải là số nguyên dương");
                }
                if (storeRequest.Status <= 0)
                {
                    return BadRequest("Status không hợp lệ");
                }
                if (storeRequest.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                var rs = await _storeService.UpdateStore(id, storeRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Update Store thất bại: {ex.Message}");
            }
            
        }


        [HttpDelete()]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var rs = await _storeService.DeleteStore(id);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Xóa Store thất bại: {ex.Message}");
            }
            
        }

        [HttpGet("api/stores/count")]
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
