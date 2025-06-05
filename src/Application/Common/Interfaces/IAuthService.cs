// src/Application/Common/Interfaces/IAuthService.cs
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null);
    Task<bool> UserExistsAsync(string email);
}

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