// ===== src/Application/Account/Register/RegisterUserHandler.cs =====
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

namespace _Net6CleanArchitectureQuizzApp.Application.Account.Commands.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserModel, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;

        public RegisterUserHandler(
            UserManager<User> userManager,
            IIdentityService identityService)
        {
            _userManager = userManager;
            _identityService = identityService;
        }

        public async Task<Result> Handle(RegisterUserModel request, CancellationToken cancellationToken)
        {
            // Validation de l'email unique
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result.Failure(new[] { "Un utilisateur avec cet email existe déjà." });
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email, // ✅ Important : utiliser email comme username
                Nom = request.Nom,
                Prenom = request.Prenom,
                EmailConfirmed = true,    // ✅ Confirmer directement
                LockoutEnabled = false    // ✅ Éviter les blocages pour les tests
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToArray();
                return Result.Failure(errors);
            }

            return Result.Success(user.Id);
        }
    }
}