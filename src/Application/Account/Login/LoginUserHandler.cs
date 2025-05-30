// ===== src/Application/Account/Login/LoginUserHandler.cs =====
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;

public class LoginHandler : IRequestHandler<LoginModel, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginHandler(
        UserManager<User> userManager,
        IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginModel request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🔍 LOGIN ATTEMPT - Email: {request.Email}");

        // ✅ Chercher par email ET par username
        var user = await _userManager.FindByEmailAsync(request.Email);
        Console.WriteLine($"🔍 User found by email: {user != null}");

        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.Email);
            Console.WriteLine($"🔍 User found by username: {user != null}");
        }

        if (user == null)
        {
            Console.WriteLine("❌ User not found");
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        Console.WriteLine($"🔍 User details - ID: {user.Id}, Email: {user.Email}, UserName: {user.UserName}");

        // ✅ Log pour debugging
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        Console.WriteLine($"🔍 Password valid: {isPasswordValid}");

        if (!isPasswordValid)
        {
            Console.WriteLine("❌ Invalid password");
            throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
        }

        // ✅ Vérifier que l'utilisateur n'est pas bloqué
        if (await _userManager.IsLockedOutAsync(user))
        {
            Console.WriteLine("❌ Account locked out");
            throw new UnauthorizedAccessException("Compte temporairement bloqué");
        }

        var roles = await _userManager.GetRolesAsync(user);
        (string token, DateTime expiry) = _tokenGenerator.GenerateToken(user);

        Console.WriteLine("✅ Login successful");

        return new AuthResponse(
            Token: token,
            Expiry: expiry,
            Nom: user.Nom,
            Prenom: user.Prenom,
            UserName: user.UserName,
            Email: user.Email
        );
    }
}
