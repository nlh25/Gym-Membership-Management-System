using FluentValidation;
using GMMS.Database.AppDbContextModels;
using GMMS.Domain.Features.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GMMS.Domain.Features.Auth;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly IValidator<LoginRequestModel> _loginValidator;
    private readonly IValidator<ChangePasswordRequestModel> _changePasswordValidator;

    public AuthService(
        AppDbContext db,
        IConfiguration configuration,
        IValidator<LoginRequestModel> loginValidator,
        IValidator<ChangePasswordRequestModel> changePasswordValidator)
    {
        _db = db;
        _configuration = configuration;
        _loginValidator = loginValidator;
        _changePasswordValidator = changePasswordValidator;
    }

    public Result<LoginResponseModel> Login(LoginRequestModel request)
    {
        var validationResult = _loginValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<LoginResponseModel>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            var user = _db.TblUsers
                .FirstOrDefault(x => !x.IsDeleted && x.UserName == request.UserName && x.IsActive);

            if (user == null)
            {
                return new Result<LoginResponseModel>
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new Result<LoginResponseModel>
                {
                    IsSuccess = false,
                    Message = "Invalid username or password."
                };
            }

            var tokenResult = GenerateToken(user);
            if (!tokenResult.IsSuccess)
            {
                return new Result<LoginResponseModel>
                {
                    IsSuccess = false,
                    Message = tokenResult.Message
                };
            }

            var session = new TblUserSession
            {
                UserId = user.UserId,
                LoginTime = DateTime.UtcNow,
                ExpiredAt = tokenResult.Data!.ExpiresAt,
                IsExpired = false
            };

            _db.TblUserSessions.Add(session);
            _db.SaveChanges();

            return new Result<LoginResponseModel>
            {
                IsSuccess = true,
                Message = "Login successful.",
                Data = tokenResult.Data
            };
        }
        catch (Exception ex)
        {
            return new Result<LoginResponseModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public Result<bool> ChangePassword(int userId, ChangePasswordRequestModel request)
    {
        var validationResult = _changePasswordValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
            };
        }

        try
        {
            var user = _db.TblUsers
                .FirstOrDefault(x => !x.IsDeleted && x.UserId == userId && x.IsActive);

            if (user == null)
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return new Result<bool>
                {
                    IsSuccess = false,
                    Message = "Current password is incorrect."
                };
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.MustChangePassword = false;
            user.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return new Result<bool>
            {
                IsSuccess = true,
                Message = "Password changed successfully.",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Result<bool>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    private Result<LoginResponseModel> GenerateToken(TblUser user)
    {
        try
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];
            var jwtExpiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");

            if (string.IsNullOrEmpty(jwtKey))
            {
                return new Result<LoginResponseModel>
                {
                    IsSuccess = false,
                    Message = "JWT key is not configured."
                };
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(jwtExpiryMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("MustChangePassword", user.MustChangePassword.ToString().ToLower())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new Result<LoginResponseModel>
            {
                IsSuccess = true,
                Data = new LoginResponseModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Role = user.Role,
                    Token = tokenString,
                    MustChangePassword = user.MustChangePassword,
                    ExpiresAt = expiresAt
                }
            };
        }
        catch (Exception ex)
        {
            return new Result<LoginResponseModel>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
