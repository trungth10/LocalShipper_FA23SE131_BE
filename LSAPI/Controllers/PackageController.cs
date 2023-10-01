using LocalShipper.Service.DTOs.Request;
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
        public async Task<ActionResult<List<PackageResponse>>> GetPackage(int batchId, int id, int status, int actionId, int typeId, string customerName, string customerAddress, string customerPhome, string custommerEmail, decimal totalPrice)
        {
            var package = await _packageService.GetPackage(batchId, id, status, actionId, typeId, customerName, customerAddress, customerPhome, custommerEmail, totalPrice);
            return Ok(package);
        }
        [HttpPost()]
        public async Task<ActionResult<PackageResponse>> PostPackage(PackageRequest request)
        {
            var rs = await _packageService.CreatePackage(request);
            return Ok(rs);
        }
        [HttpPut()]
        public async Task<ActionResult<PackageResponse>> PutPackage(int id, PackageRequest packageRequest)
        {
            var rs = await _packageService.UpdatePackage(id, packageRequest);
            return Ok(rs);
        }

        [HttpDelete()]
        public async Task<ActionResult<PackageResponse>> DeletePackage(int id)
        {
            var rs = await _packageService.DeletePackage(id);
            return Ok(rs);
        }
    }
}
