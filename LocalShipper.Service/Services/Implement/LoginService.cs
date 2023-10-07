using AutoMapper;
using LocalShipper.Data.Models;

using LocalShipper.Data.UnitOfWork;
using LocalShipper.Service.DTOs.Response;
using LocalShipper.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalShipper.Service.Services.Implement
{
    public class LoginService : ILoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
     
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        public LoginService(IMapper mapper, IUnitOfWork unitOfWork,IConfiguration configuration,ILogger<LoginService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
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
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .Where(a => a.Email == email && a.Password == password)
                    .Include(a => a.Role)
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
        };

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtAuth:Key"]));
                var issuer = _configuration["JwtAuth:Issuer"];
                var audience = _configuration["JwtAuth:Audience"];

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

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


        public async Task<string> GetUserRoleFromAccessTokenAsync(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(accessToken);

                // Lấy danh sách các Claims từ AccessToken
                var claims = token.Claims;

                // Tìm Claim có kiểu là "role"
                var roleClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

                if (roleClaim != null)
                {
                    var userRole = roleClaim.Value;
                    return userRole;
                }
                else
                {
                    return null; // Không tìm thấy vai trò
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user role from AccessToken.");
                return null; // Xảy ra lỗi
            }
        }

    }
}