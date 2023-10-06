using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LSAPI.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet()]
        public async Task<ActionResult<List<RoleResponse>>> GetRole(int? id, string? name, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber <= 0)
                {
                    return BadRequest("Số trang phải là số nguyên dương");
                }

                if (pageSize.HasValue && pageSize <= 0)
                {
                    return BadRequest("Số phần tử trong trang phải là số nguyên dương");
                }
                if (id < 0)
                {
                    return BadRequest("Id không hợp lệ");
                }
                var rs = await _roleService.GetRole(id, name, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy role");
            }
        }

        [HttpGet("api/roles/count")]
        public async Task<ActionResult<RoleResponse>> GetCountRole()
        {
            try
            {

                var rs = await _roleService.GetTotalRoleCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-role")]
        public async Task<ActionResult<RoleResponse>> CreateRole([FromBody] RegisterRoleRequest request)
        {
            try
            {
                var rs = await _roleService.CreateRole(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<RoleResponse>> UpdateRole(int id, PutRoleRequest roleRequest)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _roleService.UpdateRole(id, roleRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật role thất bại: {ex.Message}");
            }
        }

        //[HttpDelete()]
        //public async Task<ActionResult<MessageResponse>> DeleteRole(int id)
        //{
        //    try
        //    {

        //        var response = await _roleService.DeleteRole(id);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Xóa role thất bại: {ex.Message}");
        //    }
        //}
    }
}
