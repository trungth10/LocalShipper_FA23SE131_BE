using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using LocalShipper.Data.Models;
using LocalShipper.Service.Services.Implement;

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
        public async Task<ActionResult<TransactionResponse>> GetTransport(int id, int orderId)
        {
            try
            {
                var rs = await _transactionService.GetTransaction(id, orderId);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giao dịch");
            }
        }

        [HttpGet("transactions.json")]
        public async Task<ActionResult<List<TransactionResponse>>> GetAll(int? walletId, string? transactionMethod)
        {
            try
            {
                var rs = await _transactionService.GetListTransaction(walletId, transactionMethod);
                return Ok(rs);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không tìm thấy giao dịch");
            }
        }
    }
}
