
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pizza_API.Data;
using Pizza_API.Entities;

namespace Pizza_API.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public RefreshTokenService(ApplicationDbContext dbContext, IJwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        // ValidateAndRotateAsync
        public async Task<(string JwtToken, string RefreshToken)?> ValidateAndRotateAsync(string token)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var refreshToken = await _dbContext.RefreshTokens
                    .FirstOrDefaultAsync(rt =>
                        rt.Token == token &&
                        !rt.IsRevoked &&
                        rt.ExpiresOnUtc > DateTime.UtcNow);

                if (refreshToken == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                _dbContext.RefreshTokens.Remove(refreshToken);
                await _dbContext.SaveChangesAsync();

                var user = await _dbContext.Users.FindAsync(refreshToken.UserId);
                if (user == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                var jwt = await _jwtTokenService.GenerateAccessTokenAsync(user);

                // Remove existing tokens for this user
                var existingTokens = _dbContext.RefreshTokens
                    .Where(rt => rt.UserId == user.Id);
                _dbContext.RefreshTokens.RemoveRange(existingTokens);
                await _dbContext.SaveChangesAsync();

                // Create new refresh token
                var newToken = new RefreshToken
                {
                    Token = GenerateSecureToken(),
                    UserId = user.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<int>("JwtSettings:RefreshTokenValidityMins"))
                };

                _dbContext.RefreshTokens.Add(newToken);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return (jwt, newToken.Token);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // GenerateRefreshTokenAsync
        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Remove existing tokens
                var existingTokens = _dbContext.RefreshTokens
                    .Where(rt => rt.UserId == userId);
                _dbContext.RefreshTokens.RemoveRange(existingTokens);
                await _dbContext.SaveChangesAsync();

                // Create new token
                var token = new RefreshToken
                {
                    Token = GenerateSecureToken(),
                    UserId = userId,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpiresOnUtc = DateTime.UtcNow.AddMinutes(
                        _configuration.GetValue<int>("JwtSettings:RefreshTokenValidityMins"))
                };

                _dbContext.RefreshTokens.Add(token);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return token.Token;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // RevokeTokenAsync
        public async Task RevokeTokenAsync(string token)
        {
            try 
            { 
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedOnUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
            }
            catch
            {
                throw;
            }
        }
    }
}