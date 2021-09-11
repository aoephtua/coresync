// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.Model;
using McMaster.Extensions.CommandLineUtils;
using System;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    abstract class SecuredCommandBase : CommandBase
    {
        #region Protected Properties

        /// <summary>
        /// Gets or sets whether execution of <see cref="SecuredCommandBase"/> is valid.
        /// </summary>
        protected override bool Valid => IsValidConfiguration();

        #endregion

        #region Private Functions

        /// <summary>
        /// Gets whether <see cref="CoreSyncConfiguration"/> file exists and process is valid.
        /// </summary>
        /// <returns>
        /// Returns whether configuration is valid.
        /// </returns>
        private static bool IsValidConfiguration()
        {
            if (!CoreSyncConfiguration.FileExists)
            {
                if (Prompt.GetYesNo("Configuration file does not exists. Do you want to initialize synchronization directory?", true))
                {
                    CommandLineApplication.Execute<InitCommand>();
                }
                else
                {
                    Console.WriteLine("Info: Use '-?' to get help.");
                }

                return false;
            }

            return true;
        }

        #endregion
    }
}
