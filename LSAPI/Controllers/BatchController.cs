using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/batchs")]
    //[Authorize(Policy = "Shipper")]
    public class BatchController : Controller
    {
        private readonly IBatchService _batchService;
        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<OrderResponse>>> GetBatch(int id, int storeId, string batchName)
        {
            var rs = await _batchService.GetBatch(id, storeId, batchName);
            return Ok(rs);
        }
    }
}
