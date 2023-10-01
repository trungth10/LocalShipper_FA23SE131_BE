using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IWalletService
    {
        Task<List<WalletResponse>> GetWallet(int? id, decimal? balance);
        Task<int> GetTotalWalletCount();
        Task<WalletResponse> CreateWallet(WalletRequest request);
        Task<WalletResponse> UpdateWallet(int id, WalletRequest request);
    }
}
