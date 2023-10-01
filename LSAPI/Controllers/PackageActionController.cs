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
    [Route("api/package-actions")]
    public class PackageActionController : Controller
    {
        private readonly IPackageActionService _packageActionService;
        public PackageActionController(IPackageActionService packageActionService)
        {
            _packageActionService = packageActionService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<PackageActionResponse>>> GetPackageAction(int id, string actionType)
        {
            var rs = await _packageActionService.GetPackageAction(id,  actionType);
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<PackageActionResponse>> PostPackageAction(PackageActionRequest request)
        {
            var rs = await _packageActionService.CreatePackageAction(request);
            return Ok(rs);
        }
        [HttpPut()]
        public async Task<ActionResult<PackageActionResponse>> PutPackageAction(int id, PackageActionRequest packageActionRequest)
        {
            var rs = await _packageActionService.UpdatePackageAction(id, packageActionRequest);
            return Ok(rs);
        }

        [HttpDelete()]
        public async Task<ActionResult<PackageActionResponse>> DeletePackageAction(int id)
        {
            var rs = await _packageActionService.DeletePackageAction(id);
            return Ok(rs);
        }
    }
}
