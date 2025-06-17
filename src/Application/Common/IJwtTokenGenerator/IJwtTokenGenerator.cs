// src/Application/Common/Interfaces/IJwtTokenGenerator.cs - REMPLACER LE CONTENU
using System.Security.Claims;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    (string Token, DateTime Expiry) GenerateToken(User user);

    // AJOUTER cette surcharge pour la flexibilité
    string GenerateToken(string userId, string email, string[] roles);

    ClaimsPrincipal? GetPrincipalFromToken(string token);
}