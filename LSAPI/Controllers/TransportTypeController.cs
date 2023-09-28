using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportTypeController : Controller
    {
        private readonly ITransportTypeService _transportTypeService;

        public TransportTypeController(ITransportTypeService transportTypeService)
        {
            _transportTypeService = transportTypeService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<TransportTypeResponse>>> GetAll()
        {
            var rs = await _transportTypeService.GetAll();
            return Ok(rs);
        }
    }
}
