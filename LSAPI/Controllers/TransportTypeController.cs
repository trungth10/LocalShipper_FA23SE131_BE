using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using LocalShipper.Service.Helpers;

namespace LSAPI.Controllers
{
    [Route("api/transport-types")]
    [ApiController]

    public class TransportTypeController : ControllerBase
    {
        private readonly ITransportTypeService _transportTypeService;
        public TransportTypeController(ITransportTypeService transportTypeService)
        {
            _transportTypeService = transportTypeService;
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet()]
        public async Task<ActionResult<List<TransportTypeResponse>>> GetTransportType(int id, string transportType, int? pageNumber, int? pageSize)
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
                var rs = await _transportTypeService.GetTransportType(id, transportType, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy phương tiện");
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("api/transport-types/count")]
        public async Task<ActionResult<TransportTypeResponse>> GetCountTransportType()
        {
            try
            {

                var rs = await _transportTypeService.GetTotalTransportTypeCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }

        [Authorize(Roles = Roles.Staff + "," + Roles.Admin, AuthenticationSchemes = "Bearer")]
        [HttpPost("register-transport-type")]
        public async Task<ActionResult<TransportTypeResponse>> CreateTransportType([FromBody] RegisterTransportTypeRequest request)
        {
            try
            {
                var rs = await _transportTypeService.CreateTransportType(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = Roles.Staff + "," + Roles.Admin, AuthenticationSchemes = "Bearer")]
        [HttpPut()]
        public async Task<ActionResult<TransportTypeResponse>> UpdateTransportType(int id, [FromBody] PutTransportTypeRequest request)
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
                var response = await _transportTypeService.UpdateTransportType(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật phương tiện thất bại: {ex.Message}");
            }
        }

        [Authorize(Roles = Roles.Staff + "," + Roles.Admin, AuthenticationSchemes = "Bearer")]
        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteAccount(int id)
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
                var response = await _transportTypeService.DeleteTransportType(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa phương tiện thất bại: {ex.Message}");
            }
        }
    }
}

