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

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;           
        }

        [HttpPost("register-shipper-account")]
        public async Task<ActionResult<AccountResponse>> RegisterShipperAccount([FromBody] RegisterRequest request)
        {
            try
            {
                var rs = await _accountService.RegisterShipperAccount(request);                              
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("verify-otp")]
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

        [HttpPost("verify-otp-again")]
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
