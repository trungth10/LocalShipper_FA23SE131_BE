using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LocalShipper.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LocalShipper.Data.Models;
using LocalShipper.Service.Services.Helpers;

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
                    return Ok(new { AccessToken = dynamicResult.AccessToken, UserName = result.UserName ,FullName = result.FullName,Role = result.Role });
                }
                else
                {
                    return Unauthorized(dynamicResult.Message);
                }
            }

            return BadRequest("Invalid result.");
        }
    }
}
