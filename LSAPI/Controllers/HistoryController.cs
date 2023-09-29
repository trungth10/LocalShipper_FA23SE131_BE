using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LSAPI.Controllers
{
    [Route("api/histories")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<HistoryResponse>>> GetHistory(int id, string action, int storeId)
        {
            try
            {
                var rs = await _historyService.GetHistory(id, action, storeId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy lịch sử");
            }
        }

        [HttpGet("count.json")]
        public async Task<ActionResult<HistoryResponse>> GetCountHistory()
        {
            try
            {

                var rs = await _historyService.GetTotalHistoryCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-history")]
        public async Task<ActionResult<HistoryResponse>> CreateHistory([FromBody] RegisterHistoryRequest request)
        {
            try
            {
                var rs = await _historyService.CreateHistory(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<HistoryResponse>> UpdateHistory(int id, PutHistoryRequest historyRequest)
        {
            try
            {

                var response = await _historyService.UpdateHistory(id, historyRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật lịch sử thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteHistory(int id)
        {
            try
            {

                var response = await _historyService.DeleteHistory(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa lịch sử thất bại: {ex.Message}");
            }
        }
    }
}

