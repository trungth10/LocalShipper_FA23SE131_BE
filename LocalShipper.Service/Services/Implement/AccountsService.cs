using Castle.Core.Configuration;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTO.Request;
using LocalShipper.Service.DTO.Response;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Nest;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class AccountsService : IAccountsService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;
        public AccountsService(IUnitOfWork unitOfWork, IConfiguration config, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _appSettings = appSettings.Value;
        }
        public Task<string> AuthenticateAsync(AuthenticateModelWebAdmin model)
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> DeleteAccount(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResults<AccountResponse>> GetAccount(PagingRequest request, int brandId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> GetAccountById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AccountResponse>> GetAccountSurveyBySystemzoneId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyorResponse>> GetFreeSurveyor(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetJwt(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> GetMe(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SurveyorResponse>> GetSurveyor()
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> PostAccount(PostAccountRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PutAccountResponse> PutAccount(int id, PutAccountRequest model, int currentAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<string> SignInUserNamePass(SignInUsermamePass request)
        {
            throw new NotImplementedException();
        }

        public Task<VerifyResponse> VerifyJwtTraddeZoneMap(string jwt)
        {
            throw new NotImplementedException();
        }
    }
}