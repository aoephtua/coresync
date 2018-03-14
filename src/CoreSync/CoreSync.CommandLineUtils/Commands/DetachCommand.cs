// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "detach", Description = "Detach synchronization directory.", ThrowOnUnexpectedArgument = false)]
    class DetachCommand : SecuredCommandBase
    {
        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="SyncCommand"/>.
        /// </summary>
        protected override void Execute()
        {
            if (Prompt.GetYesNo("Do you want to detach synchronization directory? If no external directory is set, encrypted data will be deleted.", true))
            {
                CoreSyncProcessor.Detach();
            }
        }

        #endregion
    }
}
