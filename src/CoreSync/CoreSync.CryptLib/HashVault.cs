// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace CoreSync.CryptLib
{
    public static class HashVault
    {
        #region Public Functions

        /// <summary>
        /// Computes <see cref="byte[]"/> value with checksum by <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="input">
        /// Contains <see cref="string"/> value with data for hashing process.
        /// </param>
        /// <param name="hashName">
        /// Contains name of <see cref="HashAlgorithm"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> value with computed hash by <see cref="HashAlgorithm"/>.
        /// </returns>
        public static byte[] Compute(string input, string hashName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            return Compute(bytes, hashName);
        }

        /// <summary>
        /// Computes <see cref="byte[]"/> value with checksum by <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="input">
        /// Contains <see cref="byte[]"/> value with data for hashing process.
        /// </param>
        /// <param name="hashName">
        /// Contains name of <see cref="HashAlgorithm"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> value with computed hash by <see cref="HashAlgorithm"/>.
        /// </returns>
        public static byte[] Compute(byte[] input, string hashName)
        {
            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashName);

            if (algorithm != null)
            {
                byte[] hash = algorithm.ComputeHash(input);

                algorithm.Dispose();

                return hash;
            }

            return null;
        }

        /// <summary>
        /// Computes <see cref="byte[]"/> value with checksum by <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="input">
        /// Contains <see cref="Stream"/> value with data for hashing process.
        /// </param>
        /// <param name="hashName">
        /// Contains name of <see cref="HashAlgorithm"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> value with computed hash by <see cref="HashAlgorithm"/>.
        /// </returns>
        public static byte[] Compute(Stream input, string hashName)
        {
            var algorithm = (HashAlgorithm)CryptoConfig.CreateFromName(hashName);

            if (algorithm != null)
            {
                byte[] hash = algorithm.ComputeHash(input);

                algorithm.Dispose();

                return hash;
            }

            return null;
        }

        /// <summary>
        /// Computes a secure checksum using PBKDF2 with a specified <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="input">
        /// The input string to derive bytes from.
        /// </param>
        /// <param name="salt">
        /// The salt used for the PBKDF2 process.
        /// </param>
        /// <param name="cb">
        /// The number of pseudo-random key bytes to generate.
        /// </param>
        /// <returns>
        /// Returns a <see cref="byte[]"/> containing the derived key.
        /// </returns>
        public static byte[] Compute(string input, byte[] salt, int cb = 20)
        {
            const int iterations = 600000;

            return Rfc2898DeriveBytes.Pbkdf2
            (
                password: input,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: cb
            );
        }

        /// <summary>
        /// Converts an array of bytes to a string of hex digits.
        /// </summary>
        /// <param name="bytes">
        /// Contains instance of <see cref="byte[]"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> of hex digits.
        /// </returns>
        public static string GetHexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion
    }
}
