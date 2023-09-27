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
        Task<AccountResponse> RegisterShipperAccount( RegisterRequest request);
        Task<bool> VerifyOTP(string email, string otp);
        Task<bool> SendOTPAgain(string email);
    }
}
