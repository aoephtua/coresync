// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.Model;

#endregion

namespace CoreSync.Core
{
    #region Public Delegates

    /// <summary>
    /// Provides logging output object of <see cref="CoreSyncProcessor"/>.
    /// </summary>
    /// <param name="logEntry">
    /// Contains instance of <see cref="CoreSyncLogEntry"/>.
    /// </param>
    public delegate void LogDelegate(CoreSyncLogEntry logEntry);

    #endregion
}
