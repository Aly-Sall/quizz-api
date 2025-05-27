using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Identity;
using _Net6CleanArchitectureQuizzApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public (string Token, DateTime Expiry) GenerateToken(User user)
    {
        // Ensure required values aren't null
        var userId = user?.Id ?? throw new ArgumentNullException(nameof(user));
        var userEmail = user?.Email ?? throw new ArgumentNullException(nameof(user.Email));

        var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, userEmail),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

        // Safely add roles if they exist
        /*if (roles != null)
        {
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }*/

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    //public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,  // For refresh validation only
                ValidateAudience = false,
                ValidateLifetime = false  // Allow expired tokens
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

