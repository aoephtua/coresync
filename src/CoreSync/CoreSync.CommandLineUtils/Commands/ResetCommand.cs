// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "reset", Description = "Reset synchronization directory.", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
    class ResetCommand : ConfigurationCommand
    {
        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="SyncCommand"/>.
        /// </summary>
        protected override void Execute()
        {
            if (Prompt.GetYesNo("Do you want to reset synchronization directory? If no external directory is set, encrypted data will be deleted. Use 'config' command to change values.", true))
            {
                if (GetPassphrase(out string passphrase))
                {
                    CoreSyncProcessor.Reset(passphrase, EncryptedDirectory);
                }
            }
        }

        #endregion
    }
}
