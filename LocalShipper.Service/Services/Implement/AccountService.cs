using AutoMapper;
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

        //GET Single
        public async Task<AccountResponse> GetAccount(int id, string phone, string email)
        {
            var accounts = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == id || a.Phone == phone || a.Email == email);
            if (accounts == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", accounts.ToString());
            }
                var accountResponse = new AccountResponse
                {
                   Id = accounts.Id,
                   Fullname = accounts.Fullname,
                   Phone = accounts.Phone,
                   Email = accounts.Email,
                   RoleId = accounts.RoleId,
                   Active = accounts.Active,
                   FcmToken = accounts.FcmToken,
                   CreateDate = accounts.CreateDate,
                   ImageUrl = accounts.ImageUrl,
                   Password = accounts.Password,
                };

                if (accounts.Role != null)
                {
                    accountResponse.Role = new RoleResponse
                    {
                        Id = accounts.Role.Id,
                        Name = accounts.Role.Name,
                    };
                }                                               
            return accountResponse;
        }

        //GET List
        public async Task<List<AccountResponse>> GetListAccount(int? roleId = null, bool? active = null, DateTime? createDate = null)
        {
            IQueryable<Account> query = _unitOfWork.Repository<Account>().GetAll().Include(o => o.Role);

            if (roleId.HasValue)
            {
                query = query.Where(a => a.RoleId == roleId);
            }

            if (active.HasValue)
            {
                query = query.Where(a => a.Active == active);
            }

            if (createDate.HasValue)
            {
                query = query.Where(a => a.CreateDate.Date == createDate);
            }

            var accounts = await query.ToListAsync();

            
            if (accounts.Count == 0)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", accounts.ToString());
            }

            var accountResponses = accounts.Select(account => new AccountResponse
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
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }

                account.Fullname = accountRequest.FullName;
                account.Email = accountRequest.Email;                     
                account.Phone = accountRequest.Phone;            
                account.Password = accountRequest.Password;        
                account.RoleId = accountRequest.RoleId;           
                account.ImageUrl = accountRequest.ImageUrl;                
                account.FcmToken = accountRequest.Fcm_token;  
                account.Active = accountRequest.Active;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();

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
        public async Task<MessageResponse> DeleteAccount(int id)
        {
            
                var account = await _unitOfWork.Repository<Account>().GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

                if (account == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
                }          

                _unitOfWork.Repository<Account>().Delete(account);
                await _unitOfWork.CommitAsync();

            return new MessageResponse
            {
                Message = "Xóa tài khoản thành công",
            };
        }

        

    }
}
