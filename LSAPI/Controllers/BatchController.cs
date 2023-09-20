using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchController : Controller
    {
        private readonly IBatchService _batchService;
        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponse>> GetBatchById(int id)
        {
            var rs = await _batchService.GetBatchById(id);
            return Ok(rs);
        }
    }
}
