// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.Security.Cryptography;

#endregion

namespace CoreSync.CryptLib.Core
{
    public class SymmetricCoreCryptorConfiguration
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="int"/> value with the block size of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// Gets or sets <see cref="int"/> value with the key size of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public int KeySize { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CipherMode"/> of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public CipherMode CipherMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PaddingMode"/> of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public PaddingMode PaddingMode { get; set; }

        /// <summary>
        /// Gets or sets <see cref="int"/> value with iterations of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        /// Gets or sets <see cref="HashAlgorithmName"/> of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public HashAlgorithmName HashName { get; set; }

        #endregion
    }
}
