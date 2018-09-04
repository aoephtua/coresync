// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils.Attributes;
using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;
using System;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [HelpOption]
    abstract class CommandBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string"/> value with directory option of <see cref="CommandBase"/>.
        /// </summary>
        [Directory]
        [Option(Template = "-dir|--directory", Description = "Contains the base directory path. Default value is current directory path.")]
        public string Directory { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets instance of <see cref="CommandLineApplication"/>.
        /// </summary>
        protected CommandLineApplication App { get; private set; }

        /// <summary>
        /// Gets or sets whether execution of <see cref="CommandBase"/> is valid.
        /// </summary>
        protected virtual bool Valid => true;

        #endregion

        #region Protected Functions

        /// <summary>
        /// Raises on execution of <see cref="CommandLineApplication"/> instance.
        /// </summary>
        /// <param name="app">
        /// Contains instance of <see cref="CommandLineApplication"/>.
        /// </param>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        protected virtual int OnExecute(CommandLineApplication app)
        {
            App = app;

            if (!string.IsNullOrEmpty(Directory))
            {
                CoreSyncProcessor.WorkingDirectoryPath = CoreSyncProcessor.GetDataDirectory(Directory);
            }

            if (CoreSyncProcessor.LogNotification == null)
            {
                CoreSyncProcessor.LogNotification += (logEntry) =>
                {
                    Console.WriteLine(logEntry.ToString());
                };
            }

            try
            {
                if (Valid)
                {
                    Execute();
                }
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return 0;
        }

        /// <summary>
        /// Executes command functionalities of <see cref="CommandBase"/>.
        /// </summary>
        protected abstract void Execute();

        #endregion
    }
}
