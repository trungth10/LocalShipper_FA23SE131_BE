using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/packages")]
    //[Authorize(Roles = Roles.Shipper + "," + Roles.Store)]
    public class PackageController : Controller
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }
        
        [HttpGet()]
        public async Task<ActionResult<List<PackageResponse>>> GetPackage(int batchId, int id, int status, int actionId, int typeId, string customerName)
        {
            var package = await _packageService.GetPackage(batchId, id, status, actionId, typeId, customerName);
            return Ok(package);
        }


    }
}
