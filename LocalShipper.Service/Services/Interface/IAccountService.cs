using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IAccountService
    {
        Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, string? fcm_token, int? pageNumber, int? pageSize);
        Task<int> GetTotalAccountCount();
        Task<AccountResponse> RegisterShipperAccount(RegisterRequest request);
        Task<AccountResponse> UpdateAccount(int id, AccountRequest accountRequest);
        Task<AccountResponse> DeleteAccount(int id);
        Task<bool> VerifyOTP(string email, string otp);
        Task<bool> SendOTPAgain(string email);
        Task<AccountResponse> RegisterShipperPrivate(int storeId, RegisterRequest request);
    }
}
