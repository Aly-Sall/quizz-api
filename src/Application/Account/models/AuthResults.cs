using _Net6CleanArchitectureQuizzApp.Domain.Enums;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Models;

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public UserRole? UserRole { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UserId { get; set; }
    public string[]? Errors { get; set; }

    public static AuthResult Success(string? token, DateTime? expiry, string email, string? nom, string? prenom, string userId)
    {
        return new AuthResult
        {
            IsSuccess = true,
            Token = token,
            Expiry = expiry,
            Email = email,
            Nom = nom,
            Prenom = prenom,
            UserId = userId
        };
    }

    public static AuthResult Failure(string errorMessage)
    {
        return new AuthResult
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
    // SPÉCIFIER EXPLICITEMENT le namespace complet pour éviter la confusion
    public List<_Net6CleanArchitectureQuizzApp.Application.Account.Models.TestDto>? AvailableTests { get; set; }
    public string? ErrorMessage { get; set; }

    public static CandidateAccessResult Success(
        string authToken,
        DateTime expiry,
        string email,
        string? nom,
        string? prenom,
        List<_Net6CleanArchitectureQuizzApp.Application.Account.Models.TestDto> availableTests)
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

    public static implicit operator CandidateAccessResult(Common.Interfaces.CandidateAccessResult v)
    {
        throw new NotImplementedException();
    }
}