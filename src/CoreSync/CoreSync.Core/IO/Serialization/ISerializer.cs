// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;
using System.IO;

#endregion

namespace CoreSync.Core.IO.Serialization
{
    public interface ISerializer
    {
        /// <summary>
        /// Gets or sets instance of <see cref="Exception"/>.
        /// </summary>
        Exception Error { get; set; }

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> and copies bytes to <see cref="Stream"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="obj">
        /// Contains source instance of <typeparamref name="T"/>.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/>.
        /// </param>
        /// <param name="indent">
        /// Contains whether the output uses multiline format.
        /// </param>
        void Serialize<T>(T obj, Stream target, bool indent);

        /// <summary>
        /// Serializes instance of <typeparamref name="T"/> with <see cref="DataContractJsonSerializer"/> to target filename.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="obj">
        /// Contains source instance of <typeparamref name="T"/>.
        /// </param>
        /// <param name="filename">
        /// Contains <see cref="string"/> value target filename.
        /// </param>
        /// <param name="indent">
        /// Contains whether the output uses multiline format.
        /// </param>
        void Serialize<T>(T obj, string filename, bool indent);

        /// <summary>
        /// Deserializes instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="source">
        /// Contains source <see cref="Stream"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        T Deserialize<T>(Stream source);

        /// <summary>
        /// Deserializes instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Contains <see cref="Type"/> of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="source">
        /// Contains source <see cref="string"/> with JSON data or filename.
        /// </param>
        /// <param name="filename">
        /// Contains whether source <see cref="string"/> is filename.
        /// </param>
        /// <returns>
        /// Returns instance of <typeparamref name="T"/>.
        /// </returns>
        T Deserialize<T>(string source, bool filename = true);
    }
}
