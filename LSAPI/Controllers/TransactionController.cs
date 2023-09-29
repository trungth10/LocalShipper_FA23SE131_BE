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
        public async Task<ActionResult<List<TransactionResponse>>> GetTransaction(int id, string transactionMethod, int orderId, int walletId)
        {
            try
            {
                var rs = await _transactionService.GetTransaction(id, transactionMethod, orderId, walletId);
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

        [HttpGet("count.json")]
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
