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

namespace LSAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }


        [HttpPost]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Invalid request.");
            }

            var result = await _loginService.AuthenticateAsync(request.Email, request.Password);

            dynamic dynamicResult = result;

            if (dynamicResult != null)
            {
                if (dynamicResult.Success)
                {
                    return Ok(new
                    {
                        AccessToken = dynamicResult.AccessToken,
                        UserName = result.UserName,
                        FullName = result.FullName,
                       
                    });
                }
                else
                {
                    return Unauthorized(dynamicResult.Message);
                }
            }

            return BadRequest("Invalid result.");
        }

        [HttpGet("AccesstokenToRole")]
        public async Task<IActionResult> GetUserRole(string accesstoken)
        {
            try
            {
                // Lấy AccessToken từ tiêu đề Authorization
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Gọi hàm GetUserRoleFromAccessTokenAsync từ LoginService
                var userRole = await _loginService.GetUserRoleFromAccessTokenAsync(accessToken);

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
