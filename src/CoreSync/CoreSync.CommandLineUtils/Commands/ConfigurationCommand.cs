// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils.Attributes;
using McMaster.Extensions.CommandLineUtils;
using System;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    abstract class ConfigurationCommand : SecuredCommandBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string"/> value with encrypted directory option of <see cref="CommandBase"/>.
        /// </summary>
        [Directory]
        [Option(Template = "-encDir|--encryptedDirectory", Description = "Contains the encrypted directory path.")]
        public string EncryptedDirectory { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets <see cref="string"/> value with checked passphrase of <see cref="ConfigurationCommand"/>.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns whether passphrase is valid or not.
        /// </returns>
        public static bool GetPassphrase(out string passphrase)
        {
            passphrase = Prompt.GetPassword("Passphrase: ");

            if (!String.IsNullOrEmpty(passphrase))
            {
                if (passphrase != Prompt.GetPassword("Confirm: "))
                {
                    Console.WriteLine("Error: Passphrases do not match.");
                }
                else return true;
            }
            else
            {
                Console.WriteLine("Error: Passsphrase is required for this action.");
            }

            return false;
        }

        #endregion
    }
}
