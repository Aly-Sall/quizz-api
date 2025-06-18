// src/Application/Account/Commands/Login/LoginModel.cs - VERSION AVEC RÔLE
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Login;

public class LoginModel : IRequest<AuthResponseExtended>
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    public string Password { get; set; } = string.Empty;

    // ✅ NOUVEAU : Rôle utilisateur (optionnel, par défaut Candidate)
    public UserRole UserRole { get; set; } = UserRole.Candidate;
}