using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{

    [ApiController]
    [Route("api/package-actions")]
    public class PackageActionController : Controller
    {
        private readonly IPackageActionService _packageActionService;
        public PackageActionController(IPackageActionService packageActionService)
        {
            _packageActionService = packageActionService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<OrderResponse>>> GetPackageAction(int id, string actionType)
        {
            var rs = await _packageActionService.GetPackageAction(id,  actionType);
            return Ok(rs);
        }
    }
}
