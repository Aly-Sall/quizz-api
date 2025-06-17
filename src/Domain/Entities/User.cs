// src/Domain/Entities/User.cs - REMPLACER LE CONTENU EXISTANT
using Microsoft.AspNetCore.Identity;
using _Net6CleanArchitectureQuizzApp.Domain.Enums;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class User : IdentityUser<int>
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }

    // NOUVEAUX CHAMPS AJOUTÉS
    public UserRole UserRole { get; set; } = UserRole.Administrator;
    public DateTime? LastLoginDate { get; set; }

    // Navigation property pour les tokens d'accès candidat
    public ICollection<CandidateAccessToken> CandidateAccessTokens { get; set; } = new List<CandidateAccessToken>();
}