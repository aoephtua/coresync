// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils.Commands;
using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;

#endregion

namespace CoreSync.CommandLineUtils
{
    [HelpOption]
    [Command(Name = CoreSyncProcessor.ApplicationName, ThrowOnUnexpectedArgument = false)]
    [Subcommand("init", typeof(InitCommand))]
    [Subcommand("config", typeof(ConfigCommand))]
    [Subcommand("filter", typeof(FilterCommand))]
    [Subcommand("sync", typeof(SyncCommand))]
    [Subcommand("detach", typeof(DetachCommand))]
    [Subcommand("reset", typeof(ResetCommand))]
    public class Bootstraper
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="List{string}"/> with remaining arguments of <see cref="CommandLineApplication"/>.
        /// </summary>
        public List<string> RemainingArguments { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Main function that is bootstrapping the core processor when the CLI application is started.
        /// </summary>
        /// <param name="args">
        /// Contains instance of <see cref="string[]"/> with startup arguments.
        /// </param>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        public static int ExecuteApplication(string[] args) => CommandLineApplication.Execute<Bootstraper>(args);

        #endregion

        #region Private Functions

        /// <summary>
        /// Raises on execution of <see cref="CommandLineApplication"/> instance.
        /// </summary>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        private int OnExecute()
        {
            return CommandLineApplication.Execute<SyncCommand>(this.RemainingArguments?.ToArray());
        }

        #endregion
    }
}
