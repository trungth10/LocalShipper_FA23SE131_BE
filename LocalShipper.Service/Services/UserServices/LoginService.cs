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
using Microsoft.Extensions.Logging; // Thêm using này cho việc logging
using System.Linq;
using LocalShipper.Service.DTOs;

public class LoginService
{
    private readonly IGenericRepository<Account> _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginService> _logger; // Thêm logger

    public LoginService(
        IGenericRepository<Account> accountRepository,
        IConfiguration configuration,
        ILogger<LoginService> logger) // Thêm logger vào constructor
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
                .Where(a => a.Email == email)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return new AuthenticationResult { Success = false, Message = "Invalid email or password." };
            }

            if (password == account.Password)
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, account.Email)
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

                return new AuthenticationResult { Success = true, AccessToken = tokenString };
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

}
