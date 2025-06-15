using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;

    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GetUserNameAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user?.UserName ?? "";
    }

    public async Task<bool> IsInRoleAsync(int userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(int userId, string policyName)
    {
        return true; // Implémentation basique
    }

    public async Task<(Result Result, int userId)> CreateUserAsync(string userName, string password)
    {
        var user = new User
        {
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (Result.Success(), user.Id);
        }

        var errors = result.Errors.Select(e => e.Description);
        return (Result.Failure(errors.ToArray()), -1);
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Result.Success();
            }

            var errors = result.Errors.Select(e => e.Description);
            return Result.Failure(errors.ToArray());
        }

        return Result.Failure("User not found");
    }
}
