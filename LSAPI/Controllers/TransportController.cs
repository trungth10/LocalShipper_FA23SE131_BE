using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportController : Controller
    {
        private readonly ITransportService _transportService;
        public TransportController(ITransportService transportService)
        {
            _transportService = transportService;
        }

        [HttpPost("register-transport")]
        public async Task<ActionResult<TransportResponse>>AddTransport([FromBody] TransportRequest request)
        {
            try
            {
                var rs = await _transportService.AddTransport(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
