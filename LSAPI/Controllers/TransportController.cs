using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [Route("api/transports")]
    [ApiController]
    public class TransportController : ControllerBase
    {

        private readonly ITransportService _transportService;
        public TransportController(ITransportService transportService)
        {
            _transportService = transportService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<TransportResponse>>> GetTransport(int? id, int? typeId, string? licencePlate, string? transportColor,
                                                        string? transportImage, string? transportRegistration, int? pageNumber, int? pageSize)
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
                var rs = await _transportService.GetTransport(id, typeId, licencePlate, transportColor, transportImage, transportRegistration, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy phương tiện");
            }
        }

        //[HttpGet("transports.json")]
        //public async Task<ActionResult<List<TransportResponse>>> GetAll(int? typeId, string? transportColor)
        //{
        //    try
        //    {
        //        var rs = await _transportService.GetListTransport(typeId, transportColor);
        //        return Ok(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Không tìm thấy phương tiện");
        //    }
        //}

        [HttpGet("api/transports/count")]
        public async Task<ActionResult<TransportResponse>> GetCountTransport()
        {
            try
            {

                var rs = await _transportService.GetTotalTransportCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-transport")]
        public async Task<ActionResult<TransportResponse>> CreateTransport([FromBody] RegisterTransportRequest request)
        {
            try
            {
                if (request.TypeId <= 0)
                {
                    return BadRequest("TypeId phải là số nguyên dương");
                }
                var rs = await _transportService.CreateTransport(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<TransportResponse>> UpdateAccount(int id, [FromBody] PutTransportRequest request)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id < 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (request.TypeId <= 0)
                {
                    return BadRequest("TypeId phải là số nguyên dương");
                }
                var response = await _transportService.UpdateTransport(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật phương tiện thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<TransportResponse>> DeleteAccount(int id)
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
                var response = await _transportService.DeleteTransport(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"De-active phương tiện thất bại: {ex.Message}");
            }
        }
    }
}