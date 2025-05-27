using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Net6CleanArchitectureQuizzApp.Infrastructure.Settings;
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string? Secret { get; init; }
    public int TokenLifetimeMinutes { get; init; }
    //public int RefreshTokenLifetimeDays { get; init; }
    public string? Issuer { get; init; }
    public  string? Audience { get; init; }
}
