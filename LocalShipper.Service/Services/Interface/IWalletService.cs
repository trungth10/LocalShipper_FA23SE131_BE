using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Helpers.Momo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IWalletService
    {
        Task<List<WalletResponse>> GetWallet(int? id, decimal? balance, int? shipperId, int? type, int? pageNumber, int? pageSize);
        Task<int> GetTotalWalletCount();
        Task<WalletResponse> CreateWallet(WalletRequest request);
        Task<WalletResponse> UpdateWallet(int id, WalletRequest request);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(WalletTransactionPayment model);
    }
}
