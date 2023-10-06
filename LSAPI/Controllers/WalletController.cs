﻿using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using LocalShipper.Service.DTOs.Request;

namespace LSAPI.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IWalletTransactionService _walletTransService;

        public WalletController(IWalletService walletService, IWalletTransactionService walletTransService)
        {
            _walletService = walletService;
            _walletTransService = walletTransService;
        }

        [HttpGet("wallet-transaction")]
        public async Task<ActionResult<WalletTransactionResponse>> GetWalletTrans(int id, string transactionType, int fromWallet, 
            int toWallet, decimal amount, int? pageNumber, int? pageSize)
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
                var rs = await _walletTransService.GetWalletTrans(id, transactionType, fromWallet, toWallet, amount, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem giao dịch thất bại: {ex.Message}");
            }

        }

        [HttpGet("count-wallet-transaction")]
        public async Task<ActionResult<WalletTransactionResponse>> GetCountWalletTrans()
        {
            try
            {
                var rs = await _walletTransService.GetTotalWalletTransCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }
        }

        [HttpPost("create-wallet-transaction")]
        public async Task<ActionResult<WalletTransactionResponse>> CreateWalletTrans([FromBody] WalletTransactionRequest request)
        {
            try
            {
                if (request.FromWalletId <= 0)
                {
                    return BadRequest("FromWalletId phải là số nguyên dương");
                }
                if (request.ToWalletId <= 0)
                {
                    return BadRequest("ToWalletId phải là số nguyên dương");
                }
                if (request.Amount <= 0)
                {
                    return BadRequest("Amount phải > 0");
                }
                var rs = await _walletTransService.CreateWalletTrans(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Giao dịch thất bại: {ex.Message}");
            }
        }

        [HttpPut("update-wallet-transaction")]
        public async Task<ActionResult<WalletTransactionResponse>> UpdateWalletTrans(int id, WalletTransactionRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (request.FromWalletId <= 0)
                {
                    return BadRequest("FromWalletId phải là số nguyên dương");
                }
                if (request.ToWalletId <= 0)
                {
                    return BadRequest("ToWalletId phải là số nguyên dương");
                }
                if (request.Amount <= 0)
                {
                    return BadRequest("Amount phải > 0");
                }
                var response = await _walletTransService.UpdateWalletTrans(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật giao dịch thất bại: {ex.Message}");
            }
        }

        [HttpDelete("delete-wallet-transaction")]
        public async Task<ActionResult<MessageResponse>> DeleteWalletTrans(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _walletTransService.DeleteWalletTrans(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa giao dịch thất bại: {ex.Message}");
            }
        }

        [HttpGet()]
        public async Task<ActionResult<WalletResponse>> GetWallet(int id, decimal balance, int? pageNumber, int? pageSize)
        {
            try
            {
                var rs = await _walletService.GetWallet(id, balance, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem vi thất bại: {ex.Message}");
            }

        }

        [HttpGet("count")]
        public async Task<ActionResult<WalletResponse>> GetCountWallet()
        {
            try
            {
                var rs = await _walletService.GetTotalWalletCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }
        }

        [HttpPost()]
        public async Task<ActionResult<WalletResponse>> CreateWallet([FromBody] WalletRequest request)
        {
            try
            {
                if (request.Balance < 0)
                {
                    return BadRequest("Balance phải >= 0");
                }
                var rs = await _walletService.CreateWallet(request);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Tạo ví thất bại: {ex.Message}");
            }
        }

        [HttpPut()]
        public async Task<ActionResult<WalletResponse>> UpdateAccount(int id, [FromBody] WalletRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (request.Balance < 0)
                {
                    return BadRequest("Balance phải >= 0");
                }
                var response = await _walletService.UpdateWallet(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật ví thất bại: {ex.Message}");
            }
        }
    }
}
