// src/Application/Account/Models/AuthModels.cs - VERSION CORRIGÉE AVEC TESTDTO
using _Net6CleanArchitectureQuizzApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Models;

// ✅ MODÈLE POUR LA REQUÊTE DE LOGIN AVEC RÔLE
public class LoginWithRoleRequest
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le rôle utilisateur est requis")]
    public UserRole UserRole { get; set; }
}

// ✅ MODÈLE POUR LA RÉPONSE D'AUTHENTIFICATION ÉTENDUE AVEC TESTDTO
public class AuthResponseExtended
{
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public UserRole UserRole { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Message { get; set; }
    public bool RequiresEmailInvitation { get; set; }
    public bool IsSuccess { get; set; } = true;

    // ✅ LISTE DES TESTS DISPONIBLES POUR LES CANDIDATS
    public List<TestDto> AvailableTests { get; set; } = new List<TestDto>();
}

// ✅ MODÈLE POUR LA REQUÊTE DE LOGIN STANDARD
public class LoginRequest
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    public string Password { get; set; } = string.Empty;
}

// ✅ MODÈLE POUR LA RÉPONSE D'AUTHENTIFICATION STANDARD
public class AuthResponse
{
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public int UserId { get; set; }
    public bool IsSuccess { get; set; } = true;
    public string? Message { get; set; }
}

// ✅ MODÈLE POUR L'ENREGISTREMENT
public class RegisterRequest
{
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est requis")]
    [MinLength(4, ErrorMessage = "Le mot de passe doit contenir au moins 4 caractères")]
    public string Password { get; set; } = string.Empty;

    public string? Nom { get; set; }
    public string? Prenom { get; set; }
}

// ✅ MODÈLE POUR LA VÉRIFICATION D'ACCÈS CANDIDAT
public class CandidateAccessRequest
{
    [Required(ErrorMessage = "Le token d'accès est requis")]
    public string AccessToken { get; set; } = string.Empty;
}

// ✅ MODÈLE POUR LA RÉPONSE D'ACCÈS CANDIDAT
public class CandidateAccessResponse
{
    public bool? IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? CandidateName { get; set; }
    public string? TestName { get; set; }
    public string? ErrorMessage { get; set; }
    public List<TestDto> AvailableTests { get; set; } = new List<TestDto>();
}