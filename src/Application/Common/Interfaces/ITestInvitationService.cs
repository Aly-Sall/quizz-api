// src/Application/Common/Interfaces/ITestInvitationService.cs - NOUVEAU FICHIER
namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface ITestInvitationService
{
    Task SendCandidateInvitationEmailAsync(string email, string? nom);
    Task<bool> ValidateInvitationTokenAsync(string token);
    Task<string> GenerateInvitationTokenAsync(string email);
}