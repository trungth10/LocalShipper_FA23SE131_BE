using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;
using Microsoft.AspNetCore.Identity;
using LocalShipper.Service.Helpers;
using Microsoft.AspNetCore.Http;
using LocalShipper.Service.Exceptions;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.Services.Implement;
using MailKit.Search;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LSAPI.Controllers
{
    [ApiController]
    
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;           
        }

        [HttpGet("api/accounts")]
        public async Task<ActionResult<AccountResponse>> GetAccount(int id, string phone, string email, int role, string fcm_token, int? pageNumber, int? pageSize)
        {
            try
            {
                if (pageNumber.HasValue && pageNumber < 0)
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
                var rs = await _accountService.GetAccount(id, phone, email, role, fcm_token, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem tài khoản thất bại: {ex.Message}");
            }
            
        }

        

        [HttpGet("api/accounts/count")]
        public async Task<ActionResult<AccountResponse>> GetCountAccount()
        {
            try
            {

                var rs = await _accountService.GetTotalAccountCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("api/accounts/register-shipper-account")]
        public async Task<ActionResult<AccountResponse>> RegisterShipperAccount([FromBody] RegisterRequest request)
        {
            try
            {

                var regex = new Regex("^[0-9]+$");
                var regex2 = new Regex("^[a-zA-Z]+$");
                if (!regex.IsMatch(request.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }if (!regex2.IsMatch(request.Phone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                var rs = await _accountService.RegisterShipperAccount(request);                              
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("store/api/accounts/add-shipper")]
        public async Task<ActionResult<AccountResponse>> RegisterShippePrivate(int storeId ,[FromBody] RegisterRequest request)
        {
            try
            {
                var regex = new Regex("^[0-9]+$");
                var regex2 = new Regex("^[a-zA-Z]+$");
                if (!regex.IsMatch(request.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }
                if (!regex2.IsMatch(request.Phone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }
                var rs = await _accountService.RegisterShipperPrivate(storeId,request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("api/accounts")]
        public async Task<ActionResult<AccountResponse>> UpdateAccount(int id, [FromBody] AccountRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var regex = new Regex("^[0-9]+$");
                var regex2 = new Regex("^[a-zA-Z]+$");
                if (!regex.IsMatch(request.FullName))
                {
                    return BadRequest("Tên không hợp lệ");
                }
                if (!regex2.IsMatch(request.Phone))
                {
                    return BadRequest("Số điện thoại không hợp lệ");
                }

                var response = await _accountService.UpdateAccount(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật tài khoản thất bại: {ex.Message}");
            }
        }

        [HttpDelete("api/accounts")]
        public async Task<ActionResult<AccountResponse>> DeleteAccount(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _accountService.DeleteAccount(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa tài khoản thất bại: {ex.Message}");
            }
        }



        [HttpPost("api/accounts/verify-otp")]
        public async Task<ActionResult> VerifyOTP(string email, string otp)
        {
            try
            {
                bool isVerified = await _accountService.VerifyOTP(email, otp);

                if (isVerified)
                {
                    return Ok("OTP verified successfully.");
                }
                else
                {
                    return BadRequest("Invalid OTP.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("api/accounts/verify-otp-again")]
        public async Task<ActionResult> SendOTPAgain(string email)
        {
            try
            {
                bool isVerified = await _accountService.SendOTPAgain(email);

                if (isVerified)
                {
                    return Ok("OTP send successfully.");
                }
                else
                {
                    return BadRequest("Fail!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
