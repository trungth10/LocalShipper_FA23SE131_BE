using LocalShipper.Service.DTO.Request;
using LocalShipper.Service.DTO.Response;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Interface
{
    public interface IAccountsService
    {
        Task<string> AuthenticateAsync(AuthenticateModelWebAdmin model);

        Task<VerifyResponse> VerifyJwtTraddeZoneMap(string jwt);

        Task<AccountResponse> GetAccountById(int id);
        Task<PagedResults<AccountResponse>> GetAccount(PagingRequest request, int brandId);
        Task<AccountResponse> DeleteAccount(int id);
        Task<AccountResponse> PostAccount(PostAccountRequest request);

        Task<PutAccountResponse> PutAccount(int id, PutAccountRequest model, int currentAccountId);

        Task<List<AccountResponse>> GetAccountSurveyBySystemzoneId(int id);

        Task<List<SurveyorResponse>> GetSurveyor();

        Task<List<SurveyorResponse>> GetFreeSurveyor(int id);
        Task<string> GetJwt(int accountId);
        Task<string> SignInUserNamePass(SignInUsermamePass request);
        Task<AccountResponse> GetMe(int accountId);

        // Task<string> AuthenticateAsync(SignInUserNamePass request);

    }
}
