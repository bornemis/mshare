﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Request {
    /// <summary>
    /// Represents a PasswordUpdate request
    /// </summary>
    public class PasswordUpdate {
        /// <summary>
        /// Email of the user as in SMTP standard
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Name to be displayed
        /// </summary>
        public String Token { get; set; }

        /// <summary>
        /// Unhashed password
        /// </summary>
        public String Password { get; set; }
    }

    /// <summary>
    /// Validator object for NewUser data class
    /// </summary>
    public class PasswordUpdateValidator : AbstractValidator<PasswordUpdate> {

        /// <summary>
        /// Initializese the validator object
        /// </summary>
        public PasswordUpdateValidator() {

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .Must(x => x.Any(char.IsLower))
                .WithMessage("Must have a lower case letter!")
                .Must(x => x.Any(char.IsUpper))
                .WithMessage("Must have an upper case letter!")
                .Must(x => x.Any(char.IsDigit))
                .WithMessage("Must have at least one digit!")
                .MinimumLength(6);

            RuleFor(x => x.Token)
                .Length(40);
        }
    }
}