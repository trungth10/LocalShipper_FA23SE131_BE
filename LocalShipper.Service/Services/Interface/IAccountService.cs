using LocalShipper.Data.Models;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
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

        Task<string> UploadImageToFirebase(int accountId, IFormFile image);
        Task<AccountResponse> UpdateAccount(int id, AccountRequest accountRequest);
        Task<AccountResponse> DeleteAccount(int id);
        Task<bool> VerifyOTP(string email, string otp);
        Task<bool> SendOTPAgain(string email);
        Task<AccountResponse> RegisterShipperPrivate(int storeId, RegisterRequest request);
        Task<AccountResponse> SendMailForgotPassword(string email);
        Task<bool> VerifyForgotPassword(string email, string otp);

        Task<string> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<string> ChangePasswordOfForget(string email, string newPassword);
        Task SendTrackingOrder(string email, string trackingNumber, int orderId);
        Task SendNotificationToStore(string email, string trackingNumber);
        Task SendNotificationToStoreWaiting(string email, string trackingNumber);
        Task<AccountResponse> ActiveShipperFromStaff(int accountId, int zoneId);
        Task<string> SendOTPWallet(string email);
    }
}
