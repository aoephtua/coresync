// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.IO;
using System;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public abstract class CoreSyncEncryptableBase<T> : CoreSyncSerializeableBase<T>
    {
        #region Public Functions

        /// <summary>
        /// Encrypts and saves instance of <typeparamref name="T"/> to file system.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public virtual bool EncryptInstance(string passphrase)
        {
            if (!string.IsNullOrEmpty(TargetFileName))
            {
                try
                {
                    var configuration = CoreSyncConfiguration.SingletonInstance;

                    return DataProcessor.Encrypt(base.Serialize(), TargetFileName, passphrase);
                }
                catch (Exception e)
                {
                    CoreSyncProcessor.Log(e);
                }
            }

            return false;
        }

        /// <summary>
        /// Decrypts instance of <typeparamref name="T"/> from file system.
        /// </summary>
        /// <param name="targetFileName">
        /// Contains <see cref="string"/> value with target file name.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        public static T DecryptInstance(string targetFileName, string passphrase)
        {
            if (!string.IsNullOrEmpty(targetFileName))
            {
                try
                {
                    var configuration = CoreSyncConfiguration.SingletonInstance;

                    return Deserialize(DataProcessor.Decrypt(targetFileName, passphrase));
                }
                catch (Exception e)
                {
                    CoreSyncProcessor.Log(e);
                }
            }

            return default;
        }

        #endregion
    }
}
