// src/Application/Account/Models/LoginWithRoleModels.cs - Version complète
using _Net6CleanArchitectureQuizzApp.Domain.Enums;


public class LoginWithRoleRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole UserRole { get; set; }
}

public class VerifyCandidateAccessRequest
{
    public string Token { get; set; } = null!;
}

public class AuthResponseExtended
{
    public string? Token { get; set; }
    public DateTime? Expiry { get; set; }
    public UserRole? UserRole { get; set; }
    public string? Email { get; set; }
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? UserName { get; set; }
    public string? Message { get; set; }
    public bool RequiresEmailInvitation { get; set; }
    public List<TestDto>? AvailableTests { get; set; }
    public bool IsSuccess { get; internal set; }
}

public class TestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Duration { get; set; }
}