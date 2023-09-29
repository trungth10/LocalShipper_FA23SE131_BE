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
        Task<AccountResponse> GetAccount(int id, string phone, string email);
        Task<List<AccountResponse>> GetListAccount(int? roleId, bool? active, DateTime? createDate);
        Task<int> GetTotalAccountCount();
        Task<AccountResponse> RegisterShipperAccount(RegisterRequest request);
        Task<AccountResponse> UpdateAccount(int id, AccountRequest accountRequest);
        Task<MessageResponse> DeleteAccount(int id);
        Task<bool> VerifyOTP(string email, string otp);
        Task<bool> SendOTPAgain(string email);
    }
}
