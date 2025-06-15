// src/Infrastructure/Services/AuthService.cs - VERSION FINALE
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result> CreateUserAsync(string userName, string email, string password)
    {
        var user = new User
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return Result.Success(user.Id);
        }

        var errors = result.Errors.Select(e => e.Description).ToArray();
        return Result.Failure(errors);
    }

    public Task<AuthResult> LoginAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResult> RegisterAsync(string email, string password, string? nom = null, string? prenom = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExistsAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            return user;
        }
        return null;
    }
}