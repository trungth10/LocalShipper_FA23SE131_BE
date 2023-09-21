using LocalShipper.Data.Models;
using LocalShipper.Data.Repository;
using LocalShipper.Service.DTOs.Response;
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
    public class LoginService
    {
        private readonly IGenericRepository<Account> _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        public LoginService(
            IGenericRepository<Account> accountRepository,
            IConfiguration configuration,
            ILogger<LoginService> logger)
        {
            _accountRepository = accountRepository;
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
                var account = await _accountRepository
                    .GetAll()
                    .Where(a => a.Email == email).Include(a => a.Role)
                    .FirstOrDefaultAsync();

                if (account == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                if (password == account.Password)
                {
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
                        Message = "Invalid email or password."
                    };
                }
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

    }
}
