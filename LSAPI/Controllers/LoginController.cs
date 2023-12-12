﻿using Microsoft.AspNetCore.Mvc;
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
using LocalShipper.Service.DTOs.Response;
using Azure;
using System.Net.Http;
using MailKit.Net.Smtp;
using LocalShipper.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace LSAPI.Controllers
{

    [ApiController]
    [Route("api/logins")]
    public class LoginController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoginService _iloginService;
             private readonly IEmailService _iEmailService;
        private readonly HttpClient _httpClient;
        private readonly string _azureFunctionUrl;

        public LoginController(ILoginService loginService, IEmailService iEmailService, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _iloginService = loginService;
            _iEmailService = iEmailService;
            _httpClient = httpClientFactory.CreateClient();
            _azureFunctionUrl = "https://mailchecklc.azurewebsites.net/api/Function1?code=Hr0yLNj5kw0jlmcu3KKGlb-JZYpbhN20mmSQDGAzSOvVAzFu3vh_YA==";
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

        [HttpPost("otp")]
        public async Task<IActionResult> LoginOTP(string email)
        {
            if (email == null || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid request.");
            }

            var result = await _iloginService.LoginOTP(email);

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

        [HttpGet("verify-otp")]
        public async Task<ActionResult> VerifyOTP(string email, string otp)
        {
            try
            {

                var rs = await _iloginService.VerifyLoginOTP(email, otp);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Thất bại: {ex.Message}");
            }
        }

        [HttpGet("accesstoken-to-role")]
        public async Task<IActionResult> GetUserRole()
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

        [Authorize(Roles = Roles.Shipper, AuthenticationSchemes = "Bearer")]
        [HttpGet("accesstoken-to-infoshipper")]
        
        public async Task<IActionResult> GetAccountInfoShipperFromAccessToken()
        {
            try
            {
                // Lấy AccessToken từ tiêu đề Authorization
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Gọi hàm GetAccountInfoFromAccessTokenAsync từ LoginService
                var userInfo = await _iloginService.GetAccountShipperInfoFromAccessTokenAsync(accessToken);

                if (userInfo != null)
                {
                    return Ok(userInfo);
                }
                else
                {
                    return BadRequest("Failed to get user info from AccessToken.");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, "An error occurred while getting user info.");
            }
        }

        [Authorize(Roles = Roles.Store, AuthenticationSchemes = "Bearer")]
        [HttpGet("accesstoken-to-infostore")]

        public async Task<IActionResult> GetAccountInfoStoreFromAccessToken()
        {
            try
            {
                // Lấy AccessToken từ tiêu đề Authorization
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Gọi hàm GetAccountInfoFromAccessTokenAsync từ LoginService
                var userInfo = await _iloginService.GetAccountStoreInfoFromAccessTokenAsync(accessToken);

                if (userInfo != null)
                {
                    return Ok(userInfo);
                }
                else
                {
                    return BadRequest("Failed to get user info from AccessToken.");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, "An error occurred while getting user info.");
            }
        }
        [Authorize(Roles = Roles.Admin + "," + Roles.Staff, AuthenticationSchemes = "Bearer")]
        [HttpGet("accesstoken-to-info")]

        public async Task<IActionResult> GetAccountInfoFromAccessToken()
        {
            try
            {
                // Lấy AccessToken từ tiêu đề Authorization
                var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Gọi hàm GetAccountInfoFromAccessTokenAsync từ LoginService
                var userInfo = await _iloginService.GetAccountInfoFromAccessTokenAsync(accessToken);

                if (userInfo != null)
                {
                    return Ok(userInfo);
                }
                else
                {
                    return BadRequest("Failed to get user info from AccessToken.");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return StatusCode(500, "An error occurred while getting user info.");
            }
        }

        //[Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("check-email")]
        public async Task<ActionResult<EmailValidationResponse>> CheckEmailValidity(string email)
        {
            try
            {
                var emailExit = await _unitOfWork.Repository<Account>().GetAll().FirstOrDefaultAsync(a => a.Email == email);
                if(emailExit.Email == email)
                {
                    return BadRequest("email đã tồn tại");
                }
                // Call the Azure Function with the provided URL and key
                HttpResponseMessage functionResponse = await _httpClient.GetAsync($"{_azureFunctionUrl}&email={Uri.EscapeDataString(email)}");

                functionResponse.EnsureSuccessStatusCode(); // Ensure the response is successful.

                string responseBody = await functionResponse.Content.ReadAsStringAsync();
                // Parse and return the response as needed
                return Ok(responseBody);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling Azure Function: {ex.Message}");
            }
        }

    }
}
