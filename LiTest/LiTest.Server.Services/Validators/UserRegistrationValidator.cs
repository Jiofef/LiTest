using FluentValidation;
using LiTest.Server.Infrastructure.Data;
using LiTest.Shared.Core.Community;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Services.Validators
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistrationDto>
    {
        public UserRegistrationValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required")
            .MinimumLength(3).WithMessage("Login is too short")
            .Matches(@"^[a-zA-Z0-9_\-]+$").WithMessage("Only letters, digits, _ and - allowed")
            .WithErrorCode("invalid_login_format");

            RuleFor(x => x.Nickname)
                .MaximumLength(50).WithMessage("Nickname is too long").WithErrorCode("nickname_too_long");

            RuleFor(up => up.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required").WithErrorCode("password_is_empty")
                .MinimumLength(8).WithMessage("Minimum 8 characters").WithErrorCode("too_short_password")
                .MaximumLength(72).WithMessage("Maximum 72 characters").WithErrorCode("too_long_password")
                .Matches(@"^\S+$").WithMessage("Password cannot contain spaces").WithErrorCode("password_contains_spaces")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit").WithErrorCode("password_needs_digit");
        }
    }
}
