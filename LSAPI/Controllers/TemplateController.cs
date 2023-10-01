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
    [Route("api/templates")]
    public class TemplateController : Controller
    {

        private readonly ITemplateService _templateService;
        public TemplateController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<TemplateResponse>>> GetTemplate(int id, string templateName)
        {
            var rs = await _templateService.GetTemplate(id, templateName);
            return Ok(rs);
        }
        [HttpPost()]
        public async Task<ActionResult<TemplateResponse>> PostTemplate(TemplateRequest request)
        {
            var rs = await _templateService.CreateTemplate(request);
            return Ok(rs);
        }
        [HttpPut()]
        public async Task<ActionResult<TemplateResponse>> PutTemplate(int id, TemplateRequest templateRequest)
        {
            var rs = await _templateService.UpdateTemplate(id, templateRequest);
            return Ok(rs);
        }

        [HttpDelete()]
        public async Task<ActionResult<TemplateResponse>> DeleteTemplate(int id)
        {
            var rs = await _templateService.DeleteTemplate(id);
            return Ok(rs);
        }
    }
}
