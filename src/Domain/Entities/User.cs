using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace _Net6CleanArchitectureQuizzApp.Domain.Entities;


public class User : IdentityUser<int>
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    // ✅ Supprimer cette ligne - Email existe déjà dans IdentityUser
    // public string? Email { get; set; }
}
