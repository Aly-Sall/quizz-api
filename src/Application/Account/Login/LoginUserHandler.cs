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
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        (string token, DateTime expiry) = _tokenGenerator.GenerateToken(user);
        //string refreshToken = _tokenGenerator.GenerateRefreshToken();

        return new AuthResponse(
            Token: token,
            //RefreshToken: refreshToken,
            Expiry: expiry,
            Nom: user.Nom,
            Prenom: user.Prenom,
            UserName: user.UserName,
            Email: user.Email
            //Role: roles.FirstOrDefault(),
            //UserId: user.UserId

        );
    }
}

