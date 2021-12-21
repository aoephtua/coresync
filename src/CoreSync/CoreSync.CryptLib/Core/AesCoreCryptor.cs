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
            Iterations = 2500
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
        /// Creates instance of <see cref="Aes"/> by encryption key and salt.
        /// </summary>
        /// <param name="key">
        /// Contains <see cref="string"/> value with encryption key of <see cref="Aes"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with encryption salt of <see cref="Aes"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="Aes"/> containing encryption values.
        /// </returns>
        protected override SymmetricAlgorithm CreateSymmetricAlgorithm(string key, byte[] salt)
        {
            var k = new Rfc2898DeriveBytes(key, salt, Configuration.Iterations);

            var aes = Aes.Create();

            aes.BlockSize = Configuration.BlockSize;
            aes.KeySize = Configuration.KeySize;
            aes.Mode = Configuration.CipherMode;
            aes.Padding = Configuration.PaddingMode;

            aes.Key = k.GetBytes(aes.KeySize / 8);
            aes.IV = k.GetBytes(aes.BlockSize / 8);

            return aes;
        }

        #endregion
    }
}
