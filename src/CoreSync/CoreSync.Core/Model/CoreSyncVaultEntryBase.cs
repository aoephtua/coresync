// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public abstract class CoreSyncVaultEntryBase<T> : CoreSyncEncryptableBase<T>
    {
        #region Protected Properties

        /// <summary>
        /// Contains <see cref="string"/> value with target file name of <see cref="CoreSyncVaultEntryBase{T}"/>.
        /// </summary>
        protected override string TargetFileName => CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(this.RelativeFileName);

        /// <summary>
        /// Contains <see cref="string"/> value with relative file name of <see cref="CoreSyncVaultEntryBase{T}"/>.
        /// </summary>
        protected abstract string RelativeFileName { get; }

        #endregion
    }
}
