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
        //private readonly IValidator<RegisterUserModel> _validator;

        public RegisterUserHandler(
            UserManager<User> userManager,
            IIdentityService identityService)
            //IValidator<RegisterUserModel> validator)
        {
            _userManager = userManager;
            _identityService = identityService;
            //_validator = validator;
        }

        public async Task<Result> Handle(RegisterUserModel request, CancellationToken cancellationToken)
        {
            // Validate the request
            /////var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            //if (!validationResult.IsValid)
            //{
             //   return Result.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            //}

            // Create new user object
            var user = new User
            {
                Email = request.Email,
                Nom = request.Nom,
                Prenom = request.Prenom
            };

            // Create the user in the identity system
            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded)
            {
                var errors = identityResult.Errors.Select(e => e.Description).ToArray();
                return Result.Failure(errors);
            }


            return Result.Success();
        }
    }
}
