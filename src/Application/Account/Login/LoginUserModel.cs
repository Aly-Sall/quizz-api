using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;
public record LoginModel(
    string Email,
    string Password) : IRequest<AuthResponse>; // Changed to return AuthResponse directly

public record AuthResponse(
    string Token,
    //string RefreshToken,
    DateTime Expiry,
    string? Nom,
    string? Prenom,
    string? UserName,
    string? Email
    //string? Role,
    );
