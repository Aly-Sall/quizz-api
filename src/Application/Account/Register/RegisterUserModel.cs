using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using MediatR;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register;
public record RegisterUserModel : IRequest<Result>
{
    // public int Id { get; set; }  
    public string? Nom { get; set; } 
    public string? Prenom { get; set; } 
    public string? Email { get; set; } 
    public string? Password { get; set; }
    //public string? Role { get; set; } 
}