// src/Infrastructure/Services/IdentityService.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<User> userManager,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<string> GetUserNameAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(int userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<(Result Result, int userId)> CreateUserAsync(string userName, string password)
    {
        var user = new User
        {
            UserName = userName,
            Email = userName,
            Nom = "Default",
            Prenom = "User"
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (Result.Success(), user.Id);
        }

        var errors = result.Errors.Select(e => e.Description).ToArray();
        return (Result.Failure(errors), 0);
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Result.Success();
        }

        var errors = result.Errors.Select(e => e.Description).ToArray();
        return Result.Failure(errors);
    }
}