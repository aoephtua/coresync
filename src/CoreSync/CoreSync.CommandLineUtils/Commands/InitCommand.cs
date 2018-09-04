// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "init", Description = "Initialize synchronization directory.", ThrowOnUnexpectedArgument = false)]
    class InitCommand : ConfigurationCommand
    {
        #region Protected Properties

        /// <summary>
        /// Gets or sets whether execution of <see cref="InitCommand"/> is valid.
        /// </summary>
        protected override bool Valid => true;

        #endregion

        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="SyncCommand"/>.
        /// </summary>
        protected override void Execute()
        {
            if (GetPassphrase(out string passphrase))
            {
                CoreSyncProcessor.Initialize(passphrase, EncryptedDirectory);
            }
        }

        #endregion
    }
}
