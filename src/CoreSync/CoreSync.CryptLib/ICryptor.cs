// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.IO;
using System.Text;

#endregion

namespace CoreSync.CryptLib
{
    public interface ICryptor
    {
        /// <summary>
        /// Encryptes <see cref="string"/> value.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="string"/> value with data for encryption.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for encryption.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for encryption
        /// </param>
        /// <param name="encoding">
        /// Contains instance of <see cref="Encoding"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> value with encrypted data.
        /// </returns>
        byte[] Encrypt(string data, string passphrase, byte[] salt, Encoding encoding);

        /// <summary>
        /// Encryptes <see cref="Stream"/> value.
        /// </summary>
        /// <param name="source">
        /// Contains source <see cref="Stream"/> for encryption.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/> for encryption.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for encryption.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="string"/> value with salt for encryption.
        /// </param>
        void Encrypt(Stream source, Stream target, string passphrase, byte[] salt);

        /// <summary>
        /// Decryptes <see cref="string"/> value.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="byte[]"/> value with encrypted data for decryption.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for decryption.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for decryption.
        /// </param>
        /// <param name="encoding">
        /// Contains instance of <see cref="Encoding"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with decrypted data.
        /// </returns>
        string Decrypt(byte[] data, string passphrase, byte[] salt, Encoding encoding);

        /// <summary>
        /// Decryptes <see cref="string"/> value.
        /// </summary>
        /// <param name="source">
        /// Contains source <see cref="Stream"/> for decryption.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/> for decryption.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for decryption.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for decryption.
        /// </param>
        void Decrypt(Stream source, Stream target, string passphrase, byte[] salt);
    }
}
