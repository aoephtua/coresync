// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "config", Description = "Configure synchronization directory.", ThrowOnUnexpectedArgument = false)]
    [Subcommand("pw", typeof(PassphraseCommand))]
    class ConfigCommand : ConfigurationCommand
    {
        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="SyncCommand"/>.
        /// </summary>
        protected override void Execute()
        {
            CoreSyncProcessor.Configure(base.EncryptedDirectory);
        }

        #endregion

        #region Subcommands

        [Command(Description = "Change master vault passphrase.")]
        class PassphraseCommand : SecuredCommandBase
        {
            #region Protected Functions

            /// <summary>
            /// Executes command functionalities of <see cref="PassphraseCommand"/>.
            /// </summary>
            protected override void Execute()
            {
                if (ConfigurationCommand.GetPassphrase(out string passphrase))
                {
                    CoreSyncProcessor.SetPassphrase(passphrase);
                }
            }

            #endregion
        }

        #endregion
    }
}
