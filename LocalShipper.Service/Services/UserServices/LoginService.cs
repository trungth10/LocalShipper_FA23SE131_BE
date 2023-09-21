using LocalShipper.Data.Models;
using LocalShipper.Data.Repository;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using LocalShipper.Service.DTOs;
using LocalShipper.Service.DTOs.Response;
using System.Collections.Generic;

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

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return new AuthenticationResult { Success = false, Message = "Email and password are required." };
        }

        try
        {
            var account = await _accountRepository
                .GetAll()
                .Where(a => a.Email == email).Include(a => a.Role)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return new AuthenticationResult { Success = false, Message = "Invalid email or password." };
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

                return new AuthenticationResult { Success = true, AccessToken = tokenString, UserName = account.Email, FullName = account.Fullname, Role = account.Role.Name };
            }
            else
            {
                return new AuthenticationResult { Success = false, Message = "Invalid email or password." };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during authentication.");
            return new AuthenticationResult { Success = false, Message = "An error occurred during authentication." };
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
