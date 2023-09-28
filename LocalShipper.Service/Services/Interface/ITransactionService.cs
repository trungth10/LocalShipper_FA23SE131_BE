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
        Task<TransactionResponse> GetTransaction(int id, int orderId);

        Task<List<TransactionResponse>> GetListTransaction(int? walletId, string? transactionMethod);
    }
}
