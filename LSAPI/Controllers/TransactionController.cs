using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using LocalShipper.Data.Models;
using LocalShipper.Service.Services.Implement;
using LocalShipper.Service.DTOs.Request;
using System.Text.RegularExpressions;

namespace LSAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        [HttpGet()]
        public async Task<ActionResult<List<TransactionResponse>>> GetTransaction(int id, string transactionMethod, int orderId, int walletId, decimal amount, int? pageNumber, int? pageSize)
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
                var rs = await _transactionService.GetTransaction(id, transactionMethod, orderId, walletId, amount, pageNumber, pageSize);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giao dịch");
            }
        }

        //[HttpGet("transactions.json")]
        //public async Task<ActionResult<List<TransactionResponse>>> GetAll(int? walletId, string? transactionMethod)
        //{
        //    try
        //    {
        //        var rs = await _transactionService.GetListTransaction(walletId, transactionMethod);
        //        return Ok(rs);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Không tìm thấy giao dịch");
        //    }
        //}

        [HttpGet("api/transactions/count")]
        public async Task<ActionResult<TransactionResponse>> GetCountTransaction()
        {
            try
            {

                var rs = await _transactionService.GetTotalTransactionCount();
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xem count thất bại: {ex.Message}");
            }

        }


        [HttpPost("register-transaction")]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction([FromBody] RegisterTransactionRequest request)
        {
            try
            {
                if (request.OrderId <= 0)
                {
                    return BadRequest("OrderId phải là số nguyên dương");
                }
                if (request.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                if (request.Amount < 0)
                {
                    return BadRequest("Amount phải >= 0");
                }
                var rs = await _transactionService.CreateTransaction(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut()]
        public async Task<ActionResult<TransactionResponse>> UpdateAccount(int id, [FromBody] PutTransactionRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                if (request.OrderId <= 0)
                {
                    return BadRequest("OrderId phải là số nguyên dương");
                }
                if (request.WalletId <= 0)
                {
                    return BadRequest("WalletId phải là số nguyên dương");
                }
                if (request.Amount < 0)
                {
                    return BadRequest("Amount phải >= 0");
                }
                var response = await _transactionService.UpdateTransaction(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cập nhật giao dịch thất bại: {ex.Message}");
            }
        }

        [HttpDelete()]
        public async Task<ActionResult<MessageResponse>> DeleteAccount(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id phải là số nguyên dương");
                }
                var response = await _transactionService.DeleteTransaction(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Xóa giao dịch thất bại: {ex.Message}");
            }
        }
    }
}
