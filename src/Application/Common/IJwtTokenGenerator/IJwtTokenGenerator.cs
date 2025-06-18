// src/Application/Common/Interfaces/IJwtTokenGenerator.cs
using System.Security.Claims;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    // Méthode principale utilisée par AuthService
    (string Token, DateTime Expiry) GenerateToken(User user);

    // Méthodes additionnelles pour flexibilité
    string GenerateToken(string userId, string email, User user);
    string GenerateToken(string userId, string email, string[] roles);

    // Méthode pour valider les tokens
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}