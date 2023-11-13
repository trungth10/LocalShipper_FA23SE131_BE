using AutoMapper;
using LocalShipper.Data.Models;

using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Exceptions;
using LocalShipper.Service.Helpers;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using MailKit.Search;

namespace LocalShipper.Service.Services.Implement
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        private readonly IEmailService _emailService;

        public LoginService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<LoginService> logger, IEmailService emailService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }


        public async Task<LoginResponse> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email and password are required."
                };
            }

            try
            {
                // CreatePasswordHash(password, out string passwordHash);
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Where(a => a.Email == email && a.Password == password)
                    .Include(a => a.Role).Include(a => a.Shippers).Include(a => a.Stores)
                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                if (account.Active == false)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Account is not active."
                    };
                }

                if (password != account.Password)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Email),
            new Claim(ClaimTypes.Role, account.Role.Name),
            new Claim("sub", account.Id.ToString())
        };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuth:Key"]));
                var issuer = _configuration["JwtAuth:Issuer"];
                var audience = _configuration["JwtAuth:Audience"];

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                if (token.ValidTo < DateTime.UtcNow)
                {
                    
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Access token đã hết hạn.",
                        StatusCode = 400 
                    };
                }

                return new LoginResponse
                {
                    Success = true,
                    AccessToken = tokenString,
                    Id = account.Id,
                    UserName = account.Email,
                    FullName = account.Fullname,
                    Role = account.Role.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication."
                };
            }
        }

        private void CreatePasswordHash(string password, out string passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = hmac.ComputeHash(passwordBytes);

                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }



        public async Task<LoginResponse> LoginOTP(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Email are required."
                };
            }

            try
            {
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Where(a => a.Email == email)
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync();



                if (account == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email"
                    };
                }

                if (account.Active == false)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Account is not active."
                    };
                }

                Random random = new Random();
                int otp = random.Next(1000, 9999);

                account.FcmToken = otp.ToString();
                await _unitOfWork.Repository<Account>().Update(account, account.Id);
                await _unitOfWork.CommitAsync();

                string subject = "Local Shipper - Login OTP";
                string content = $"Vui lòng nhập OTP: {otp} để xác thực.";

                var message = new Message(new List<string> { email }, subject, content);
                _emailService.SendEmail(message);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Kiểm tra email để xác thực bằng cách nhập OTP"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during authentication.");
                return new LoginResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication."
                };
            }
        }

        public async Task<LoginResponse> VerifyLoginOTP(string email, string otp)
        {
            var account = await _unitOfWork.Repository<Account>()
                     .GetAll()
                     .Where(a => a.Email == email)
                     .Include(a => a.Role)
                     .FirstOrDefaultAsync();
            if (account == null)
            {

                throw new CrudException(HttpStatusCode.NotFound, "Email không tồn tại", email.ToString());
            }
            if (otp != account.FcmToken)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Sai mã OTP", email.ToString());
            }

            if (account != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Email),
            new Claim(ClaimTypes.Role, account.Role.Name),
            new Claim("sub", account.Id.ToString())
        };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuth:Key"]));
                var issuer = _configuration["JwtAuth:Issuer"];
                var audience = _configuration["JwtAuth:Audience"];

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                if (token.ValidTo < DateTime.UtcNow)
                {
                   
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Access token đã hết hạn.",
                        StatusCode = 400
                    };
                }

                return new LoginResponse
                {
                    Success = true,
                    AccessToken = tokenString,
                    Id = account.Id,
                    UserName = account.Email,
                    FullName = account.Fullname,
                    Role = account.Role.Name
                };

            }
            else
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tài khoản không tồn tại."
                };
            }

        }


        public async Task<string> GetUserRoleFromAccessTokenAsync(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);

               
                var claims = token.Claims;

               
                var roleClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                if (roleClaim != null)
                {
                    var userRole = roleClaim.Value;
                    return userRole;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user role from AccessToken.");
                return null;
            }
        }

        public async Task<AccountInfoShippperResponse> GetAccountShipperInfoFromAccessTokenAsync(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);

                
                var claims = token.Claims;

               
                var accountId = GetAccountIdFromClaims(claims);
                if (accountId.HasValue)
                {

                    var account = await _unitOfWork.Repository<Account>()
                        .GetAll()
                        .Include(a => a.Shippers)
                        .Include(a => a.Stores)
                        .Include(a => a.Role)
                        .FirstOrDefaultAsync(a => a.Id == accountId);

                    if (account != null)
                    {
                       
                            var accountResponse = new AccountInfoShippperResponse
                            {
                                Role = account.Role.Name,
                                Fullname = account.Fullname,
                                Phone = account.Phone,
                                Email = account.Email,
                                RoleId = account.RoleId,
                                ShipperId = account.Shippers.Single().Id,
                                AddressShipper = account.Shippers.Single().AddressShipper,
                                TransportId = account.Shippers.Single().TransportId,
                                ZoneId = account.Shippers.Single().ZoneId,
                                //WalletId = account.Shippers.Single().WalletId,
                                AccountId = account.Id,
                                Active = account.Active,
                                FcmToken = account.FcmToken,
                                CreateDate = account.CreateDate,
                                ImageUrl = account.ImageUrl,
                                Password = account.Password,
                            };
                            return accountResponse;
                        }
                       

                    
                }

                return null; // Không tìm thấy tài khoản hoặc thông tin
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return null; // Xảy ra lỗi
            }
        }
        public async Task<AccountInfoStoreResponse> GetAccountStoreInfoFromAccessTokenAsync(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);

                
                var claims = token.Claims;

                var accountId = GetAccountIdFromClaims(claims);
                if (accountId.HasValue)
                {

                    var account = await _unitOfWork.Repository<Account>()
                        .GetAll()
                        .Include(a => a.Shippers)
                        .Include(a => a.Stores)
                        .Include(a => a.Role)
                        .FirstOrDefaultAsync(a => a.Id == accountId);

                    if (account != null)
                    {

                        var accountResponse = new AccountInfoStoreResponse
                        {
                            Role = account.Role.Name,
                            Fullname = account.Fullname,
                            Phone = account.Phone,
                            Email = account.Email,
                            RoleId = account.RoleId,
                            StoreId = account.Stores.Single().Id,
                            StoreAddress = account.Stores.Single().StoreAddress,
                            StoreDescription = account.Stores.Single().StoreDescription,
                            StoreName = account.Stores.Single().StoreName,
                            ZoneId = account.Stores.Single().ZoneId,
                            WalletId = account.Stores.Single().WalletId,
                            OpenTime = account.Stores.Single().OpenTime,
                            CloseTime = account.Stores.Single().CloseTime,
                            Status = account.Stores.Single().Status,
                            TemplateId= account.Stores.Single().TemplateId,
                            AccountId = account.Id,                       
                            FcmToken = account.FcmToken,
                            CreateDate = account.CreateDate,
                            ImageUrl = account.ImageUrl,
                            Password = account.Password,
                        };
                        return accountResponse;
                    }



                }

                return null; 
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }

        public async Task<AccountInfoResponse> GetAccountInfoFromAccessTokenAsync(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);


                var claims = token.Claims;

                var accountId = GetAccountIdFromClaims(claims);
                if (accountId.HasValue)
                {

                    var account = await _unitOfWork.Repository<Account>()
                        .GetAll()
                        .Include(a => a.Shippers)
                        .Include(a => a.Stores)
                        .Include(a => a.Role)
                        .FirstOrDefaultAsync(a => a.Id == accountId);

                    if (account != null)
                    {

                        var accountResponse = new AccountInfoResponse
                        {
                            Role = account.Role.Name,
                            Id= account.Id,
                            Fullname = account.Fullname,
                            Phone = account.Phone,
                            Email = account.Email,
                            RoleId = account.RoleId,                            
                            FcmToken = account.FcmToken,
                            CreateDate = account.CreateDate,
                            ImageUrl = account.ImageUrl,
                            Password = account.Password,
                        };
                        return accountResponse;
                    }



                }

                return null;
            }
            catch (Exception ex)
            {

                return null;
            }
        }


        private int? GetAccountIdFromClaims(IEnumerable<Claim> claims)
        {
            // Thay đổi logic ở đây để lấy ID tài khoản từ danh sách Claims
            foreach (var claim in claims)
            {
                if (claim.Type == "sub")
                {
                    if (int.TryParse(claim.Value, out int accountId))
                    {
                        return accountId;
                    }
                }
            }
            return null; // Không tìm thấy ID tài khoản
        }





    }
}