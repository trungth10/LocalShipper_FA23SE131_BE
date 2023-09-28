﻿using LocalShipper.Service.DTOs.Response;
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

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;           
        }

        [HttpGet()]
        public async Task<ActionResult<AccountResponse>> GetSingleAccount(int id, string phone, string email)
        {
            try
            {

                var rs = await _accountService.GetAccount(id, phone, email);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem tài khoản thất bại: {ex.Message}");
            }
            
        }

        [HttpGet("accounts.json")]
        public async Task<ActionResult<List<AccountResponse>>> GetListAccount(int? roleId, bool? active, DateTime? createDate)
        {
            try
            {

                var rs = await _accountService.GetListAccount(roleId, active, createDate);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem tài khoản thất bại: {ex.Message}");
            }

        }

        [HttpGet("count.json")]
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

        [HttpPut()]
        public async Task<ActionResult<AccountResponse>> UpdateAccount(int id, [FromBody] AccountRequest request)
        {
            try
            {

                var response = await _accountService.UpdateAccount(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật đơn hàng thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteAccount(int id)
        {
            try
            {

                var response = await _accountService.DeleteAccount(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa đơn hàng thất bại: {ex.Message}");
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
