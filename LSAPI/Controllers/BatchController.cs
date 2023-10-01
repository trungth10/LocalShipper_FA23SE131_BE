using LocalShipper.Service.DTOs.Request;
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
        public async Task<ActionResult<List<BatchResponse>>> GetBatch(int id, int storeId, string batchName)
        {
            var rs = await _batchService.GetBatch(id, storeId, batchName);
            return Ok(rs);
        }

        [HttpPost()]
        public async Task<ActionResult<BatchResponse>> PostBatch(BatchRequest request)
        {
            var rs = await _batchService.CreateBatch(request);
            return Ok(rs);
        }


        [HttpPut()]
        public async Task<ActionResult<BatchResponse>> PutBatch(int id, BatchRequest batchRequest)
        {
            var rs = await _batchService.UpdateBatch(id, batchRequest);
            return Ok(rs);
        }


        /// <summary>
        /// Xóa một batch bằng cách cập nhật trạng thái thành 4 (DELETE).
        /// </summary>
        /// <param name="id">ID của batch cần xóa.</param>
        /// <returns>Thông tin của batch đã bị xóa.</returns>
        [HttpDelete()]
        public async Task<ActionResult<BatchResponse>> DeleteBatch(int id)
        {
            var rs = await _batchService.DeleteBatch(id);
            return Ok(rs);
        }
    }
}
