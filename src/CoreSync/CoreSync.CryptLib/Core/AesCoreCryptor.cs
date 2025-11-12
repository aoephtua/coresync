// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.Security.Cryptography;

#endregion

namespace CoreSync.CryptLib.Core
{
    public class AesCoreCryptor : SymmetricCoreCryptor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="AesCoreCryptor"/> with default <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public AesCoreCryptor() : base(new SymmetricCoreCryptorConfiguration()
        {
            BlockSize = 128,
            KeySize = 256,
            CipherMode = CipherMode.CBC,
            PaddingMode = PaddingMode.PKCS7,
            Iterations = 600000,
            HashName = HashAlgorithmName.SHA256
        }) { }

        /// <summary>
        /// Initializes a new instance of <see cref="AesCoreCryptor"/>.
        /// </summary>
        /// <param name="configuration">
        /// Contains instance of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </param>
        public AesCoreCryptor(SymmetricCoreCryptorConfiguration configuration) : base(configuration) { }

        #endregion

        #region Private Functions

        /// <summary>
        /// Creates an instance of <see cref="Aes"/> using a password-based key derivation (PBKDF2).
        /// </summary>
        /// <param name="key">
        /// The password or base key string.
        /// </param>
        /// <param name="salt">
        /// The salt used in the key derivation process.
        /// </param>
        /// <returns>
        /// Returns an initialized <see cref="Aes"/> instance with derived Key and IV.
        /// </returns>
        protected override SymmetricAlgorithm CreateSymmetricAlgorithm(string key, byte[] salt)
        {
            var aes = Aes.Create();

            aes.BlockSize = Configuration.BlockSize;
            aes.KeySize = Configuration.KeySize;
            aes.Mode = Configuration.CipherMode;
            aes.Padding = Configuration.PaddingMode;

            var totalBytes = (aes.KeySize + aes.BlockSize) / 8;

            byte[] derived = Rfc2898DeriveBytes.Pbkdf2
            (
                password: key,
                salt: salt,
                iterations: Configuration.Iterations,
                hashAlgorithm: Configuration.HashName,
                outputLength: totalBytes
            );

            var keyBytes = aes.KeySize / 8;
            var ivBytes = aes.BlockSize / 8;

            aes.Key = derived[..keyBytes];
            aes.IV = derived[keyBytes..(keyBytes + ivBytes)];

            return aes;
        }

        #endregion
    }
}
