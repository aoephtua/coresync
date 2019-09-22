// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "sync", Description = "Synchronize data of current or specified directory.", ThrowOnUnexpectedArgument = false)]
    class SyncCommand : SecuredCommandBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string"/> value with subdirectory path option of <see cref="SyncCommand"/>.
        /// </summary>
        [Option(Template = "-subdir|--subdirectory", Description = "Contains the optional subdirectory path.")]
        public string SubdirectoryPath { get; set; }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="SyncCommand"/>.
        /// </summary>
        protected override void Execute() => CoreSyncProcessor.Synchronize(SubdirectoryPath);

        #endregion
    }
}
