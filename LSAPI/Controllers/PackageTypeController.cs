using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
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
        public async Task<ActionResult<List<PackageTypeResponse>>> GetPackageType(int id, string packageType)
        {
            var rs = await _packageTypeService.GetPackageType(id, packageType);
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<PackageTypeResponse>> PostPackageType(PackageTypeRequest request)
        {
            var rs = await _packageTypeService.CreatePackageType(request);
            return Ok(rs);
        }

        [HttpPut()]
        public async Task<ActionResult<PackageTypeResponse>> PutPackageType(int id, PackageTypeRequest packageTypeRequest)
        {
            var rs = await _packageTypeService.UpdatePackageType(id, packageTypeRequest);
            return Ok(rs);
        }
    }
}
