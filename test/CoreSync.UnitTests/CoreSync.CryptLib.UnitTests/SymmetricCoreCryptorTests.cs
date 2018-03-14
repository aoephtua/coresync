// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CryptLib.Core;
using System;
using Xunit;

#endregion

namespace CoreSync.CryptLib.UnitTests
{
    public class SymmetricCoreCryptorTests
    {
        #region Public Functions

        /// <summary>
        /// Generates random passphrase and checks whether <see cref="string"/> value is not <see langword="null"/> or empty.
        /// </summary>
        [Fact]
        public void GetPassphrase()
        {
            var passphrase = SymmetricCoreCryptor.GeneratePassphrase();

            Assert.False(String.IsNullOrEmpty(passphrase));
        }

        /// <summary>
        /// Generates random salt and checks whether <see cref="byte[]"/> is not empty.
        /// </summary>
        [Fact]
        public void GetSalt()
        {
            var salt = SymmetricCoreCryptor.GetSalt();

            Assert.NotEmpty(salt);
        }

        #endregion
    }
}
