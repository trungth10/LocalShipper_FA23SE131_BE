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
    [Route("api/stores")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<StoreResponse>>> GetStore(int id, string storeName, int status, int brandId, int zoneId, int walletId, int accountId)
        {
            try
            {
                var rs = await _storeService.GetStore(id, storeName, status, brandId, zoneId, walletId, accountId);
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
                var rs = await _storeService.CreateStore(request);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"tạo Store thất bại: {ex.Message}");
            }
            
        }
        [HttpPut()]
        public async Task<ActionResult<StoreResponse>> PutStore(int id, StoreRequest storeRequest)
        {
            try
            {
                var rs = await _storeService.UpdateStore(id, storeRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"update Store thất bại: {ex.Message}");
            }
            
        }
        [HttpDelete()]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
        {
            try
            {
                var rs = await _storeService.DeleteStore(id);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Xóa Store thất bại: {ex.Message}");
            }
            
        }

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
