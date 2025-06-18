// Dans votre fichier AuthResults.cs existant, ajoutez cette classe manquante :

using _Net6CleanArchitectureQuizzApp.Domain.Enums;

public class AuthResultWithRole
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public UserRole UserRole { get; set; }
    public string? ErrorMessage { get; set; }
    public string[]? Errors { get; set; }

    // ✅ Méthode existante pour admin avec token
    public static AuthResultWithRole Success(string token, DateTime expiry, string email, string nom, string prenom, UserRole userRole)
    {
        return new AuthResultWithRole
        {
            IsSuccess = true,
            Token = token,
            Expiry = expiry,
            Email = email,
            Nom = nom,
            Prenom = prenom,
            UserRole = userRole
        };
    }

    // ✅ NOUVELLE MÉTHODE : Pour candidats SANS token
    public static AuthResultWithRole SuccessWithoutToken(string email, string nom, string prenom, UserRole userRole)
    {
        return new AuthResultWithRole
        {
            IsSuccess = true,
            Token = null, // ❌ Pas de token pour les candidats
            Expiry = null, // ❌ Pas d'expiry non plus
            Email = email,
            Nom = nom,
            Prenom = prenom,
            UserRole = userRole
        };
    }

    public static AuthResultWithRole Failure(string error)
    {
        return new AuthResultWithRole
        {
            IsSuccess = false,
            ErrorMessage = error
        };
    }

    public static AuthResultWithRole Failure(string[] errors)
    {
        return new AuthResultWithRole
        {
            IsSuccess = false,
            Errors = errors,
            ErrorMessage = string.Join(", ", errors)
        };
    }
}