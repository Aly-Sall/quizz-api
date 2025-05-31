using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _Net6CleanArchitectureQuizzApp.Application.Common.Interfaces;
using _Net6CleanArchitectureQuizzApp.Application.Common.Models;
using _Net6CleanArchitectureQuizzApp.Domain.Constants;
using _Net6CleanArchitectureQuizzApp.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserModel, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly ILogger<RegisterUserHandler> _logger;

        public RegisterUserHandler(
            UserManager<User> userManager,
            IIdentityService identityService,
            ILogger<RegisterUserHandler> logger)
        {
            _userManager = userManager;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<Result> Handle(RegisterUserModel request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("🔍 REGISTRATION ATTEMPT - Email: {Email}", request.Email);

            // ✅ Validation de l'email unique
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("❌ Email already exists: {Email}", request.Email);
                return Result.Failure(new[] { "Un utilisateur avec cet email existe déjà." });
            }

            // ✅ Validation du nom d'utilisateur unique
            var existingUsername = await _userManager.FindByNameAsync(request.Email);
            if (existingUsername != null)
            {
                _logger.LogWarning("❌ Username already exists: {Email}", request.Email);
                return Result.Failure(new[] { "Un utilisateur avec ce nom d'utilisateur existe déjà." });
            }

            // ✅ Création de l'utilisateur
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email, // ✅ IMPORTANT : utiliser email comme username
                NormalizedEmail = request.Email.ToUpper(),
                NormalizedUserName = request.Email.ToUpper(),
                Nom = request.Nom,
                Prenom = request.Prenom,
                EmailConfirmed = true,    // ✅ Confirmer directement pour éviter les problèmes
                LockoutEnabled = false,   // ✅ Éviter les blocages pour les nouveaux utilisateurs
                AccessFailedCount = 0,
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = false
            };

            _logger.LogInformation("🔍 Creating user with email: {Email}", user.Email);
            _logger.LogInformation("🔍 Password length: {Length}", request.Password?.Length ?? 0);

            // ✅ Création avec mot de passe
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToArray();
                _logger.LogError("❌ Registration failed for {Email}: {Errors}",
                    request.Email, string.Join(", ", errors));
                return Result.Failure(errors);
            }

            _logger.LogInformation("✅ User created successfully: {Email} with ID: {Id}",
                user.Email, user.Id);

            return Result.Success(user.Id);
        }
    }
}
