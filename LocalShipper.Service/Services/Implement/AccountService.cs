﻿using AutoMapper;
using LocalShipper.Data.Models;
using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Request;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using MailKit.Search;


namespace LocalShipper.Service.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;



        public AccountService(IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
        }
        

        //Create Account Shipper
        public async Task<AccountResponse> RegisterShipperAccount(RegisterRequest request)
        {
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);
            
            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Email đã tồn tại", request.Email.ToString());
            }
            string otp = GenerateOTP();
            CreatePasswordHash(request.Password, out string passwordHash);
          


            Account account = new Account
            {               
                Fullname = request.FullName,
                Email = request.Email,
                Active = false,
                Phone = request.Phone,
                FcmToken = otp,
                RoleId = 5,
                Password = passwordHash
            };
            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            await SendOTPEmail(request.Email, otp);

            return new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,                                                                            
                
            };
            
        }

        public async Task<bool> SendOTPAgain (string email)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Email không tồn tại", email.ToString());        
                return false;
            }
            
            string otp = GenerateOTP();
            account.FcmToken = otp;
            _unitOfWork.Repository<Account>().Update(account, account.Id);
            await _unitOfWork.CommitAsync();
            await SendOTPEmail(email, otp);
            return true;
        }

        public string GenerateOTP()
        {
            Random random = new Random();
            int otp = random.Next(100000, 999999);
            return otp.ToString();
        }

        public async Task SendOTPEmail(string email, string otp)
        {
            string subject = "Xác thực tài khoản";
            string content = $"Mã OTP của bạn là: {otp}";

            var message = new Message(new List<string> { email }, subject, content);
            _emailService.SendEmail(message);
        }


        private void CreatePasswordHash(string password, out string passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {               
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        public async Task<bool> VerifyOTP(string email, string otp)
        {
            var account = _unitOfWork.Repository<Account>().Find(x => x.Email == email && x.FcmToken == otp);
            if (account != null)
            {
                account.Active = true;
                _unitOfWork.Repository<Account>().Update(account, account.Id);
                await _unitOfWork.CommitAsync();
                return true;
            }

            return false;
        }

        //Get Account  
        public async Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, int? role, string? fcm_token)
        {
            
            var accoutns = await _unitOfWork.Repository<Account>().GetAll()
                                                              .Include(o => o.Role)
                                                              .Where(a => id == 0 || a.Id == id)
                                                              .Where(a => string.IsNullOrWhiteSpace(phone) || a.Phone.Contains(phone))
                                                              .Where(a => string.IsNullOrWhiteSpace(email) || a.Email.Contains(email))
                                                              .Where(a => role == 0 || a.RoleId == role)
                                                              .Where(a => string.IsNullOrWhiteSpace(fcm_token) || a.FcmToken.Contains(fcm_token))
                                                              .ToListAsync();

            if (accoutns == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản không có hoặc không tồn tại", id.ToString());
            }

            var accountResponses = accoutns.Select(account => new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            }).ToList();
            return accountResponses;
        }
         

        //GET Count
        public async Task<int> GetTotalAccountCount()
        {
            var count = await _unitOfWork.Repository<Account>()
                .GetAll()
                .CountAsync();

            return count;
        }

        //UPDATE Account
        public async Task<AccountResponse> UpdateAccount(int id, AccountRequest accountRequest)
        {
            var accounts = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (accounts == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }

                accounts.Fullname = accountRequest.FullName;
                accounts.Email = accountRequest.Email;                     
                accounts.Phone = accountRequest.Phone;            
                accounts.Password = accountRequest.Password;        
                accounts.RoleId = (int)accountRequest.RoleId;           
                accounts.ImageUrl = accountRequest.ImageUrl;                
                accounts.FcmToken = accountRequest.Fcm_token;  
                accounts.Active = accountRequest.Active;

            await _unitOfWork.Repository<Account>().Update(accounts, id);
            await _unitOfWork.CommitAsync();

            var account = await _unitOfWork.Repository<Account>()
                 .GetAll()
                 .Include(o => o.Role)
                 .FirstOrDefaultAsync(a => a.Id == id);



            var updatedAccountResponse = new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            };

            return updatedAccountResponse;
        }

        //DELETE Account
        public async Task<AccountResponse> DeleteAccount(int id)
        {

            var account = await _unitOfWork.Repository<Account>()
            .GetAll()
            .Include(o => o.Role)
            .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }

            if (account.Active == false)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Tài khoản đã bị xóa rồi", id.ToString());
            }

            account.Active = false;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();

            var accountResponse = new AccountResponse
            {
                Id = account.Id,
                Fullname = account.Fullname,
                Phone = account.Phone,
                Email = account.Email,
                RoleId = account.RoleId,
                Active = account.Active,
                FcmToken = account.FcmToken,
                CreateDate = account.CreateDate,
                ImageUrl = account.ImageUrl,
                Password = account.Password,
                Role = account.Role != null ? new RoleResponse
                {
                    Id = account.Role.Id,
                    Name = account.Role.Name,
                } : null
            };

            return accountResponse;
        }

        

    }
}
