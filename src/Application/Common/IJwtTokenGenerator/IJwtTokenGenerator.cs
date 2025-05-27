using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;

namespace _Net6CleanArchitectureQuizzApp.Domain.Interfaces;
public interface IJwtTokenGenerator
{
    (string Token, DateTime Expiry) GenerateToken(User user);
    //string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromToken(string token);

}