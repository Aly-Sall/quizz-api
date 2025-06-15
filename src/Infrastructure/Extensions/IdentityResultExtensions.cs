// src/Infrastructure/Extensions/IdentityResultExtensions.cs
using Microsoft.AspNetCore.Identity;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Extensions;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var errors = result.Errors.Select(e => e.Description).ToArray();
        return Result.Failure(errors);
    }
}