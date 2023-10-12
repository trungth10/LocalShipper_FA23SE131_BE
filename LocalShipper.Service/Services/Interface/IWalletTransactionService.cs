using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IWalletTransactionService
    {
        Task<List<WalletTransactionResponse>> GetWalletTrans(int? id, string? transactionType, int? fromWallet, int? toWallet,
            decimal? amount, string? description, int? orderId, int? pageNumber, int? pageSize);
        Task<int> GetTotalWalletTransCount();
       // Task<WalletTransactionResponse> CreateWalletTrans(WalletTransactionRequest request);
        //Task<WalletTransactionResponse> UpdateWalletTrans(int id, WalletTransactionRequest request);
        Task<MessageResponse> DeleteWalletTrans(int id);
    }
}
