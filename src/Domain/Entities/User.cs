using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;

public class User : IdentityUser<int>
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }

    // ✅ Email et UserName sont déjà définis dans IdentityUser<int>
    // ✅ Pas besoin de les redéfinir
}