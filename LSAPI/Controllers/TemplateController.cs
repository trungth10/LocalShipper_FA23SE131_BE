﻿using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<ActionResult<List<TemplateResponse>>> GetTemplate(int id, string templateName, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 0)
                {
                    return BadRequest("pageNumber phải là số dương");
                }

                if (pageSize.HasValue && pageSize < 0)
                {
                    return BadRequest("pageSize phải là số dương");
                }
                if (id < 0)
                {
                    return BadRequest("id không hợp lệ");
                }


                var rs = await _templateService.GetTemplate(id, templateName, pageNumber, pageSize);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"Xem Template thất bại: {ex.Message}");
            }
            
        }
        [HttpPost()]
        public async Task<ActionResult<TemplateResponse>> PostTemplate(TemplateRequest request)
        {
            try
            {
                var rs = await _templateService.CreateTemplate(request);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"tạo Template thất bại: {ex.Message}");
            }
            
        }
        [HttpPut()]
        public async Task<ActionResult<TemplateResponse>> PutTemplate(int id, TemplateRequest templateRequest)
        {
            try
            {
                var rs = await _templateService.UpdateTemplate(id, templateRequest);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"update Template thất bại: {ex.Message}");
            }
            
        }

        [HttpDelete()]
        public async Task<ActionResult<TemplateResponse>> DeleteTemplate(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest("làm ơn hãy nhập id");
                }
                if (id <= 0)
                {
                    return BadRequest("id phải là số dương");
                }
                var rs = await _templateService.DeleteTemplate(id);
                return Ok(rs);
            }
            catch(Exception ex)
            {
                return BadRequest($"xóa Template thất bại: {ex.Message}");
            }
            
        }

        [HttpGet("count")]
        public async Task<ActionResult<TemplateResponse>> GetCountTemplate()
        {
            try
            {

                var rs = await _templateService.GetTotalTemplateCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }
    }
}
