using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{

    [ApiController]
    [Route("api/package-types")]
    public class PackageTypeController : Controller
    {
        private readonly IPackageTypeService _packageTypeService;
        public PackageTypeController(IPackageTypeService packageTypeService)
        {
            _packageTypeService = packageTypeService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<OrderResponse>>> GetPackageAction(int id, string packageType)
        {
            var rs = await _packageTypeService.GetPackageType(id, packageType);
            return Ok(rs);
        }
    }
}
