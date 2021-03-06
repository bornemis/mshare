﻿using FluentValidation;
using System;
using System.Linq;

namespace MShare_ASP.API.Request
{
    /// <summary>Represents a PasswordUpdate request</summary>
    public class PasswordUpdate
    {
        /// <summary>Email of the user as in SMTP standard</summary>
        public String Email { get; set; }

        /// <summary>Reset token</summary>
        public String Token { get; set; }

        /// <summary>Reset token</summary>
        public String OldPassword { get; set; }

        /// <summary>Unhashed password</summary>
        public String Password { get; set; }
    }

    /// <summary>Validator object for PasswordUpdate data class</summary>
    public class PasswordUpdateValidator : AbstractValidator<PasswordUpdate>
    {
        /// <summary>Initializes the validator object</summary>
        public PasswordUpdateValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().When(x => x.OldPassword == null)
                .Null().When(x => x.OldPassword != null);

            RuleFor(x => x.Password)
                .Must(x => x.Any(char.IsLower))
                .WithMessage("Must have a lower case letter!")
                .Must(x => x.Any(char.IsUpper))
                .WithMessage("Must have an upper case letter!")
                .Must(x => x.Any(char.IsDigit))
                .WithMessage("Must have at least one digit!")
                .MinimumLength(6);

            RuleFor(x => x.Token)
                .Length(40).When(x => x.OldPassword == null)
                .Null().When(x => x.OldPassword != null);

            RuleFor(x => x.OldPassword)
                .NotNull().When(x => x.Token == null)
                .Null().When(x => x.Token != null);
        }
    }
}