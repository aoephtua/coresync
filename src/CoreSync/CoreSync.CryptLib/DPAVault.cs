// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace CoreSync.CryptLib
{
    public static class DPAVault
    {
        #region Public Functions

        /// <summary>
        /// Gets <see cref="byte[]"/> with protected <see cref="string"/> value.
        /// </summary>
        /// <param name="value">
        /// Contains <see cref="string"/> value with plain text for protection.
        /// </param>
        /// <param name="salt">
        /// Contains optional <see cref="byte[]"/> with initialization vector.
        /// </param>
        /// <param name="scope">
        /// Contains value of <see cref="DataProtectionScope"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> with protected value.
        /// </returns>
        public static byte[] Protect(string value, byte[] salt = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            byte[] plainText = Encoding.UTF8.GetBytes(value);

            return ProtectedData.Protect(plainText, salt, scope);
        }

        /// <summary>
        /// Gets <see cref="string"/> with unprotected <see cref="byte[]"/> value.
        /// </summary>
        /// <param name="value">
        /// Contains <see cref="byte[]"/> with chiper text for unprotection.
        /// </param>
        /// <param name="salt">
        /// Contains optional <see cref="byte[]"/> with initialization vector.
        /// </param>
        /// <param name="scope">
        /// Contains value of <see cref="DataProtectionScope"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> with unprotected value.
        /// </returns>
        public static string Unprotect(byte[] value, byte[] salt = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] plainText = ProtectedData.Unprotect(value, salt, scope);

            return Encoding.UTF8.GetString(plainText);
        }

        #endregion
    }
}
