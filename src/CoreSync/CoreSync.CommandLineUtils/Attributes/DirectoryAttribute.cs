// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.ComponentModel.DataAnnotations;
using System.IO;

#endregion

namespace CoreSync.CommandLineUtils.Attributes
{
    class DirectoryAttribute : ValidationAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DirectoryAttribute"/>.
        /// </summary>
        public DirectoryAttribute() : base("The value for {0} must be a valid directory path.") { }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Validates if <see cref="string"/> value is a valid directory path.
        /// </summary>
        /// <param name="value">
        /// Contains <see cref="object"/> value.
        /// </param>
        /// <param name="validationContext">
        /// Contains instance of <see cref="ValidationContext">.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="ValidationResult"/>.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is null) && !Path.IsPathRooted(value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        #endregion
    }
}
