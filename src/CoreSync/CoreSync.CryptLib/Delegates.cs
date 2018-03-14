// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;

#endregion

namespace CoreSync.CryptLib
{
    #region Public Delegates

    /// <summary>
    /// Provides progress change notifications.
    /// </summary>
    /// <param name="length">
    /// Contains <see cref="long"/> value with status length.
    /// </param>
    /// <param name="cancel">
    /// Contains <see cref="bool"/> value with flag for progress cancellation.
    /// </param>
    public delegate void ProgressChangeDelegate(long length, ref bool cancel);

    /// <summary>
    /// Provides progess complete notifications.
    /// </summary>
    public delegate void CompleteDelegate();

    /// <summary>
    /// Provides progress error notifications.
    /// </summary>
    /// <param name="e">
    /// Contains instance of <see cref="Exception"/>.
    /// </param>
    public delegate void ErrorDelegate(Exception e);

    #endregion
}
