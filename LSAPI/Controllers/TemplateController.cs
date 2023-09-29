using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/templates")]
    public class TemplateController : Controller
    {

        private readonly ITemplateService _templateService;
        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<TemplateResponse>>> GetPackageAction(int id, string templateName)
        {
            var rs = await _templateService.GetTemplate(id, templateName);
            return Ok(rs);
        }
    }
}
