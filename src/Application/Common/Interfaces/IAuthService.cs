// src/Application/Common/Interfaces/IAuthService.cs - REMPLACER LE CONTENU EXISTANT
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
// Importer explicitement TestDto depuis Account.Models
using TestDtoModel = _Net6CleanArchitectureQuizzApp.Application.Account.Models.TestDto;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IAuthService
{
    // Méthodes existantes
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null);
    Task<bool> UserExistsAsync(string email);

    // NOUVELLES MÉTHODES POUR LA GESTION DES RÔLES
    Task<AuthResultWithRole> LoginWithRoleAsync(string email, string password, UserRole expectedRole);
    Task<CandidateAccessResult> VerifyCandidateAccessAsync(string accessToken);
}

// Classe AuthResult existante (ne pas modifier)
public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public int? UserId { get; set; }
    public string? ErrorMessage { get; set; }
    public string[]? Errors { get; set; }

    public static AuthResult Success(string token, DateTime expiry, string email, string? userName = null,
        string? nom = null, string? prenom = null, int? userId = null)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Token = token,
            Expiry = expiry,
            Email = email,
            UserName = userName,
            Nom = nom,
            Prenom = prenom,
            UserId = userId
        };
    }

    public static AuthResult Failure(string error)
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorMessage = error
        };
    }

    public static AuthResult Failure(string[] errors)
    {
        return new AuthResult
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

// NOUVELLES CLASSES POUR LA GESTION DES RÔLES
public class AuthResultWithRole
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public UserRole? UserRole { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthResultWithRole Success(string? token, DateTime? expiry, string email, string nom, string prenom, UserRole userRole)
    {
        return new AuthResultWithRole
        {
            IsSuccess = true,
            Token = token,
            Expiry = expiry,
            Email = email,
            Nom = nom,
            Prenom = prenom,
            UserRole = userRole
        };
    }

    public static AuthResultWithRole Failure(string errorMessage)
    {
        return new AuthResultWithRole
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public class CandidateAccessResult
{
    public bool IsSuccess { get; set; }
    public string? AuthToken { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    // Utiliser l'alias pour éviter la confusion avec d'autres TestDto
    public List<TestDtoModel>? AvailableTests { get; set; }
    public string? ErrorMessage { get; set; }

    public static CandidateAccessResult Success(string authToken, DateTime expiry, string email, string nom, string prenom, List<TestDtoModel> availableTests)
    {
        return new CandidateAccessResult
        {
            IsSuccess = true,
            AuthToken = authToken,
            Expiry = expiry,
            Email = email,
            Nom = nom,
            Prenom = prenom,
            AvailableTests = availableTests
        };
    }

    public static CandidateAccessResult Failure(string errorMessage)
    {
        return new CandidateAccessResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}