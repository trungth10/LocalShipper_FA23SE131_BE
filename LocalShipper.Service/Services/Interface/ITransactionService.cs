using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> GetTransaction(int? id, string? transactionMethod, int? orderId, int? walletId, decimal? amount, int? pageNumber, int? pageSize);

        //Task<List<TransactionResponse>> GetListTransaction(int? walletId, string? transactionMethod);

        Task<int> GetTotalTransactionCount();
        Task<TransactionResponse> CreateTransaction(RegisterTransactionRequest request);
        Task<TransactionResponse> UpdateTransaction(int id, PutTransactionRequest transactionRequest);
        Task<MessageResponse> DeleteTransaction(int id);
    }
}
