using LocalShipper.Service.DTOs.Response;
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
        public async Task<ActionResult<List<OrderResponse>>> GetPackageAction(int id, string storeName, int status, int brandId, int zoneId, int walletId, int accountId)
        {
            var rs = await _storeService.GetStore(id, storeName, status, brandId, zoneId, walletId, accountId);
            return Ok(rs);
        }
    }
}
