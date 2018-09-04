// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.Security.Cryptography;

#endregion

namespace CoreSync.CryptLib.Core
{
    public class RijndaelManagedCoreCryptor : SymmetricCoreCryptor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RijndaelManagedCoreCryptor"/> with default <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public RijndaelManagedCoreCryptor() : base(new SymmetricCoreCryptorConfiguration()
        {
            BlockSize = 128,
            KeySize = 256,
            CipherMode = CipherMode.CBC,
            PaddingMode = PaddingMode.PKCS7,
            Iterations = 2500
        }) { }

        /// <summary>
        /// Initializes a new instance of <see cref="RijndaelManagedCoreCryptor"/>.
        /// </summary>
        /// <param name="configuration">
        /// Contains instance of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </param>
        public RijndaelManagedCoreCryptor(SymmetricCoreCryptorConfiguration configuration) : base(configuration) { }

        #endregion

        #region Private Functions

        /// <summary>
        /// Creates instance of <see cref="RijndaelManaged"/> by encryption key and salt.
        /// </summary>
        /// <param name="key">
        /// Contains <see cref="string"/> value with encryption key of <see cref="RijndaelManaged"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with encryption salt of <see cref="RijndaelManaged"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="RijndaelManaged"/> containing encryption values.
        /// </returns>
        protected override SymmetricAlgorithm CreateSymmetricAlgorithm(string key, byte[] salt)
        {
            var k = new Rfc2898DeriveBytes(key, salt, Configuration.Iterations);

            var rijndaelAlg = new RijndaelManaged()
            {
                BlockSize = Configuration.BlockSize,
                KeySize = Configuration.KeySize,
                Mode = Configuration.CipherMode,
                Padding = Configuration.PaddingMode
            };

            rijndaelAlg.Key = k.GetBytes(rijndaelAlg.KeySize / 8);
            rijndaelAlg.IV = k.GetBytes(rijndaelAlg.BlockSize / 8);

            return rijndaelAlg;
        }

        #endregion
    }
}
