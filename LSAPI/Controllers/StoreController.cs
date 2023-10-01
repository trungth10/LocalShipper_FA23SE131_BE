using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
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
            var rs = await _storeService.GetStore(id, storeName, status, brandId, zoneId, walletId, accountId);
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<StoreResponse>> PostStore(StoreRequest request)
        {
            var rs = await _storeService.CreateStore(request);
            return Ok(rs);
        }
        [HttpPut()]
        public async Task<ActionResult<StoreResponse>> PutStore(int id, StoreRequest storeRequest)
        {
            var rs = await _storeService.UpdateStore(id, storeRequest);
            return Ok(rs);
        }
        [HttpDelete()]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
        {
            var rs = await _storeService.DeleteStore(id);
            return Ok(rs);
        }
    }
}
