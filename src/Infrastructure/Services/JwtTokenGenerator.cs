// src/Infrastructure/Services/JwtTokenGenerator.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
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

    // MÉTHODE PRINCIPALE : Retourne un tuple (string Token, DateTime Expiry)
    public (string Token, DateTime Expiry) GenerateToken(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Ajouter les informations utilisateur si disponibles
        if (!string.IsNullOrEmpty(user.Nom))
            claims.Add(new Claim("nom", user.Nom));
        if (!string.IsNullOrEmpty(user.Prenom))
            claims.Add(new Claim("prenom", user.Prenom));

        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);
        var tokenString = GenerateTokenInternal(claims, expiry);

        return (tokenString, expiry);
    }

    // MÉTHODE SURCHARGÉE : GenerateToken(string, string, User) - retourne string
    public string GenerateToken(string userId, string email, User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user != null)
        {
            if (!string.IsNullOrEmpty(user.Nom))
                claims.Add(new Claim("nom", user.Nom));
            if (!string.IsNullOrEmpty(user.Prenom))
                claims.Add(new Claim("prenom", user.Prenom));
        }

        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);
        return GenerateTokenInternal(claims, expiry);
    }

    // MÉTHODE SURCHARGÉE : GenerateToken(string, string, string[]) - retourne string
    public string GenerateToken(string userId, string email, string[] roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (roles != null)
        {
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var expiry = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenLifetimeMinutes);
        return GenerateTokenInternal(claims, expiry);
    }

    // MÉTHODE POUR VALIDER UN TOKEN
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    // MÉTHODE HELPER PRIVÉE pour éviter la duplication
    private string GenerateTokenInternal(List<Claim> claims, DateTime expiry)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}