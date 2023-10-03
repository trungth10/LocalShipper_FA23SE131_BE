using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
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
        public async Task<ActionResult<List<BatchResponse>>> GetBatch(int id, string batchName)
        {
            try
            {
                var rs = await _batchService.GetBatch(id, batchName);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem Batch thất bại: {ex.Message}");
            }

        }

        [HttpPost()]
        public async Task<ActionResult<BatchResponse>> PostBatch(BatchRequest request)
        {
            try
            {
                var rs = await _batchService.CreateBatch(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"thêm Batch thất bại: {ex.Message}");
            }

        }


        [HttpPut()]
        public async Task<ActionResult<BatchResponse>> PutBatch(int id, BatchRequest batchRequest)
        {
            try
            {
                var rs = await _batchService.UpdateBatch(id, batchRequest);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"cập nhật Batch thất bại: {ex.Message}");
            }

        }


        /// <summary>
        /// Xóa một batch bằng cách cập nhật trạng thái thành 4 (DELETE).
        /// </summary>
        /// <param name="id">ID của batch cần xóa.</param>
        /// <returns>Thông tin của batch đã bị xóa.</returns>
        [HttpDelete()]
        public async Task<ActionResult<BatchResponse>> DeleteBatch(int id)
        {
            try
            {
                var rs = await _batchService.DeleteBatch(id);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"xóa Batch thất bại: {ex.Message}");
            }

        }



        [HttpGet("count")]
        public async Task<ActionResult<BatchResponse>> GetCountBatch()
        {
            try
            {

                var rs = await _batchService.GetTotalBatchCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
