// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.IO;
using CoreSync.Core.IO.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public abstract class CoreSyncSerializeableBase<T>
    {
        #region Protected Properties

        /// <summary>
        /// Gets <see cref="string"/> value with target file name of <see cref="CoreSyncSerializeableBase{T}"/>.
        /// </summary>
        protected virtual string TargetFileName { get; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> to <see cref="Stream"/>.
        /// </summary>
        /// <returns>
        /// Returns instance of <see cref="Stream"/>.
        /// </returns>
        public virtual Stream Serialize()
        {
            var stream = new MemoryStream();

            JsonSerializer.SingletonInstance.Serialize<T>((T)(object)this, stream);

            if (stream != null)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> to file system.
        /// </summary>
        /// <param name="indent">
        /// Contains whether the output uses multiline format.
        /// </param>
        public virtual void SerializeToLocalFile(bool indent = false)
        {
            if (!string.IsNullOrEmpty(TargetFileName))
            {
                JsonSerializer.SingletonInstance.Serialize<T>((T)(object)this, CoreSyncProcessor.GetFullName(TargetFileName), indent);
            }
        }

        /// <summary>
        /// Deletes instance of <typeparamref name="T"/> from file system.
        /// </summary>
        /// <param name="withEmptyParentDirectories">
        /// Contains whether empty parent directories should be deleted.
        /// </param>
        /// <returns>
        /// Returns whether deltion process succeeded.
        /// </returns>
        public virtual bool DeleteInstance(bool withEmptyParentDirectories = false)
        {
            try
            {
                return DataProcessor.Delete(TargetFileName, withEmptyParentDirectories);
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return false;
        }

        #endregion

        #region Protected Functions

        /// <summary>
        /// Deserializes to instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="stream">
        /// Contains instance of <see cref="Stream"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        protected static T Deserialize(Stream stream) => JsonSerializer.SingletonInstance.Deserialize<T>(stream);

        /// <summary>
        /// Deserializes to instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="name">
        /// Contains <see cref="string"/> value with file name.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        protected static T DeserializeFromLocalFile(string fileName) => JsonSerializer.SingletonInstance.Deserialize<T>(fileName);

        #endregion
    }
}
