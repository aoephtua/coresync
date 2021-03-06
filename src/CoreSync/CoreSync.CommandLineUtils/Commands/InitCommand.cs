﻿// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;
using System;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "init", Description = "Initialize synchronization directory.", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
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
            if (!CoreSyncProcessor.IsInitialized())
            {
                if (GetPassphrase(out string passphrase))
                {
                    CoreSyncProcessor.Initialize(passphrase, EncryptedDirectory);
                }
            }
            else
            {
                Console.WriteLine("Error: Working directory already initialized.");

                CommandLineApplication.Execute<ResetCommand>();
            }
        }

        #endregion
    }
}
