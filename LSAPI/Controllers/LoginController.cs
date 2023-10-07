using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LocalShipper.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LocalShipper.Data.Models;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.DTOs.Request;
using System;
using LocalShipper.Service.Services.Interface;

namespace LSAPI.Controllers
{

    [ApiController]
    [Route("api/logins")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _iloginService;

        public LoginController(ILoginService loginService)
        {
            _iloginService = loginService;
        }


        [HttpPost]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid request.");
            }

            var result = await _iloginService.AuthenticateAsync(request.Email, request.Password);
          
            dynamic dynamicResult = result;

            if (dynamicResult != null)
            {
                if (dynamicResult.Success)
                {
                    return Ok(new
                    {
                        AccessToken = dynamicResult.AccessToken,
                        id = result.Id,
                        UserName = result.UserName,
                        FullName = result.FullName,
                        Role = result.Role,
                    });
                }
                else
                {
                    return Unauthorized(dynamicResult.Message);
                }
            }

            return BadRequest("Invalid result.");
        }

        [HttpGet("accesstoken-to-role")]
        public async Task<IActionResult> GetUserRole(string accesstoken)
        {
            try
            {
                // Lấy AccessToken từ tiêu đề Authorization
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Gọi hàm GetUserRoleFromAccessTokenAsync từ LoginService
                var userRole = await _iloginService.GetUserRoleFromAccessTokenAsync(accessToken);

                if (userRole != null)
                {
                    return Ok(new { Role = userRole });
                }
                else
                {
                    return BadRequest("Role not found in AccessToken.");
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while getting user role.");
                return StatusCode(500, "An error occurred while getting user role.");
            }
        }
    }
}
