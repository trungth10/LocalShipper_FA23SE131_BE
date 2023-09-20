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

    public class PackageController : Controller
    {
        private readonly IPackageService _packageService;
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("{batchId}")]
        public async Task<ActionResult<List<PackageResponse>>> GetPackageByBatchId(int batchId)
        {
            var package = await _packageService.GetPackageByBatchId(batchId);
            return Ok(package);
        }


    }
}
