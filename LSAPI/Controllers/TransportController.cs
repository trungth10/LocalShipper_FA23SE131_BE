using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
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
        public async Task<ActionResult<TransportResponse>> GetTransport(int id, string licencePlate)
        {
            try
            {
                var rs = await _transportService.GetTransport(id, licencePlate);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy phương tiện");
            }
        }

        [HttpGet("transports.json")]
        public async Task<ActionResult<List<TransportResponse>>> GetAll(int? typeId, string? transportColor)
        {
            try
            {
                var rs = await _transportService.GetListTransport(typeId, transportColor);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy phương tiện");
            }
        }
    }
}
