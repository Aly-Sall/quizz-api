// src/Domain/Enums/UserRole.cs - FICHIER ENUM POUR LES RÔLES
namespace _Net6CleanArchitectureQuizzApp.Domain.Enums;

/// <summary>
/// Énumération des rôles d'utilisateurs dans le système
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Candidat qui passe les tests
    /// </summary>
    Candidate = 0,

    /// <summary>
    /// Administrateur du système
    /// </summary>
    Administrator = 1
}