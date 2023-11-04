using LocalShipper.Service.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface ILoginService
    {
        Task<LoginResponse> AuthenticateAsync(string email, string password);
        Task<string> GetUserRoleFromAccessTokenAsync(string accessToken);
        Task<LoginResponse> LoginOTP(string email);
        Task<LoginResponse> VerifyLoginOTP(string email, string otp);

        Task<AccountInfoShippperResponse> GetAccountShipperInfoFromAccessTokenAsync(string accessToken);
        Task<AccountInfoStoreResponse> GetAccountStoreInfoFromAccessTokenAsync(string accessToken);
        Task<AccountInfoResponse> GetAccountInfoFromAccessTokenAsync(string accessToken);

    }
}
