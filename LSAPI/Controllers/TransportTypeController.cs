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
    [Route("api/transport-types")]
    [ApiController]
    public class TransportTypeController : ControllerBase
    {
        private readonly ITransportTypeService _transportTypeService;
        public TransportTypeController(ITransportTypeService transportTypeService)
        {
            _transportTypeService = transportTypeService;
        }



        [HttpGet()]
        public async Task<ActionResult<List<TransportTypeResponse>>> GetTransportType(int id, string transportType, int? pageNumber, int? pageSize)
        {
            try
            {
                var rs = await _transportTypeService.GetTransportType(id, transportType, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy phương tiện");
            }
        }

        [HttpGet("count")]
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

        [HttpPut()]
        public async Task<ActionResult<TransportTypeResponse>> UpdateTransportType(int id, [FromBody] PutTransportTypeRequest request)
        {
            try
            {

                var response = await _transportTypeService.UpdateTransportType(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật phương tiện thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteAccount(int id)
        {
            try
            {

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

