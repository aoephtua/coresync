// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils;

#endregion

namespace CoreSync
{
    class Program
    {
        #region Main Functions

        /// <summary>
        /// First main function that is invoked when the application is started.
        /// </summary>
        /// <param name="args">
        /// Contains instance of <see cref="string[]"/> with startup arguments.
        /// </param>
        /// <returns>
        /// Returns exit code for the current process.
        /// </returns>
        static int Main(string[] args)
        {
            return Bootstraper.ExecuteApplication(args);
        }

        #endregion
    }
}
