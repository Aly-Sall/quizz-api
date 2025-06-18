// src/Application/Common/Interfaces/IAuthService.cs - VERSION FINALE CORRIGÉE
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null);
    Task<bool> UserExistsAsync(string email);

    // ✅ MÉTHODE AVEC RÔLE
    Task<IAuthService.AuthResultWithRole> LoginWithRoleAsync(string email, string password, UserRole expectedRole);

    // ✅ MÉTHODE POUR CANDIDATS
    Task<IAuthService.CandidateAccessResult> VerifyCandidateAccessAsync(string accessToken);

    // ✅ CLASSES INTERNES POUR LES RÉSULTATS
    public class AuthResult
    {
        public bool IsSuccess { get; private set; }
        public string? Token { get; private set; }
        public DateTime? Expiry { get; private set; }
        public string? Email { get; private set; }
        public string? UserName { get; private set; }
        public string? Nom { get; private set; }
        public string? Prenom { get; private set; }
        public int UserId { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string[]? Errors { get; private set; }

        private AuthResult(bool isSuccess, string? token = null, DateTime? expiry = null,
            string? email = null, string? userName = null, string? nom = null,
            string? prenom = null, int userId = 0, string? errorMessage = null, string[]? errors = null)
        {
            IsSuccess = isSuccess;
            Token = token;
            Expiry = expiry;
            Email = email;
            UserName = userName;
            Nom = nom;
            Prenom = prenom;
            UserId = userId;
            ErrorMessage = errorMessage;
            Errors = errors;
        }

        public static AuthResult Success(string token, DateTime expiry, string email,
            string? userName, string nom, string prenom, int userId)
            => new(true, token, expiry, email, userName, nom, prenom, userId);

        public static AuthResult Failure(string errorMessage)
            => new(false, errorMessage: errorMessage);

        public static AuthResult Failure(string[] errors)
            => new(false, errors: errors);
    }

    public class AuthResultWithRole
    {
        public bool IsSuccess { get; private set; }
        public string? Token { get; private set; }
        public DateTime? Expiry { get; private set; }
        public string? Email { get; private set; }
        public string? Nom { get; private set; }
        public string? Prenom { get; private set; }
        public UserRole UserRole { get; private set; }
        public string? ErrorMessage { get; private set; }

        private AuthResultWithRole(bool isSuccess, string? token = null, DateTime? expiry = null,
            string? email = null, string? nom = null, string? prenom = null,
            UserRole userRole = UserRole.Candidate, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            Token = token;
            Expiry = expiry;
            Email = email;
            Nom = nom;
            Prenom = prenom;
            UserRole = userRole;
            ErrorMessage = errorMessage;
        }

        public static AuthResultWithRole Success(string token, DateTime expiry,
            string email, string nom, string prenom, UserRole userRole)
            => new(true, token, expiry, email, nom, prenom, userRole);

        public static AuthResultWithRole SuccessWithoutToken(string email,
            string nom, string prenom, UserRole userRole)
            => new(true, email: email, nom: nom, prenom: prenom, userRole: userRole);

        public static AuthResultWithRole Failure(string errorMessage)
            => new(false, errorMessage: errorMessage);
    }

    public class CandidateAccessResult
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? Email { get; private set; }
        public string? Nom { get; private set; }
        public string? Prenom { get; private set; }
        public string? AuthToken { get; private set; }
        public DateTime? Expiry { get; private set; }
        public List<TestDto>? AvailableTests { get; private set; }

        private CandidateAccessResult(bool isSuccess, string? errorMessage = null,
            string? email = null, string? nom = null, string? prenom = null,
            string? authToken = null, DateTime? expiry = null, List<TestDto>? availableTests = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Email = email;
            Nom = nom;
            Prenom = prenom;
            AuthToken = authToken;
            Expiry = expiry;
            AvailableTests = availableTests;
        }

        public static CandidateAccessResult Success(string email, string nom, string prenom,
            string authToken, DateTime expiry, List<TestDto>? availableTests = null)
            => new(true, null, email, nom, prenom, authToken, expiry, availableTests);

        public static CandidateAccessResult Failure(string errorMessage)
            => new(false, errorMessage: errorMessage);
    }
}