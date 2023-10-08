using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MailKit;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Authorization;
using LocalShipper.Service.Helpers;

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

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<HistoryResponse>>> GetHistory(int id, string action, int storeId, int? pageNumber, int? pageSize)
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
                    return BadRequest("Id không hợp lệ");
                }
                var rs = await _historyService.GetHistory(id, action, storeId, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy lịch sử");
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("api/histories/count")]
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


        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPost("api/histories/create-history")]
        public async Task<ActionResult<HistoryResponse>> CreateHistory([FromBody] RegisterHistoryRequest request)
        {
            try
            {
                if (request.StoreId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                var rs = await _historyService.CreateHistory(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpPut("api/histories")]
        public async Task<ActionResult<HistoryResponse>> UpdateHistory(int id, PutHistoryRequest historyRequest)
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
                if (historyRequest.StoreId <= 0)
                {
                    return BadRequest("StoreId phải là số nguyên dương");
                }
                var response = await _historyService.UpdateHistory(id, historyRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật lịch sử thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Store + "," + Roles.Staff + "," + Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpDelete("api/histories")]
        public async Task<ActionResult<MessageResponse>> DeleteHistory(int id)
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

