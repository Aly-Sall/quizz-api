// src/Application/Common/Interfaces/ICandidateInvitationService.cs - NOUVEAU FICHIER
using _Net6CleanArchitectureQuizzApp.Application.Account.Models;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;

public interface ICandidateInvitationService
{
    Task<bool> SendCandidateInvitationEmailAsync(string candidateEmail, string candidateName);
    Task<List<TestDto>> GetAvailableTestsForCandidateAsync(string candidateEmail);
}