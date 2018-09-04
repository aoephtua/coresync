// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System.IO;
using System.Text;

#endregion

namespace CoreSync.CryptLib
{
    public static class IOProcessor
    {
        #region Public Functions

        /// <summary>
        /// Writes <see cref="string"/> data to instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="byte[]"/> value for writing process.
        /// </param>
        /// <param name="target">
        /// Contains target instance of <see cref="Stream"/>.
        /// </param>
        public static void Write(byte[] data, Stream target)
        {
            try
            {
                BinaryWriter binaryWriter = new BinaryWriter(target);

                binaryWriter.Write(data);
            }
            catch { }
        }

        /// <summary>
        /// Writes <see cref="string"/> data to instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">
        /// Contains <see cref="string"/> value for writing process.
        /// </param>
        /// <param name="target">
        /// Contains target instance of <see cref="Stream"/>.
        /// </param>
        /// <param name="encoding">
        /// Contains optional instance of <see cref="Encoding"/>.
        /// </param>
        public static void Write(string source, Stream target, Encoding encoding = null)
        {
            SetEncoding(ref encoding);

            Write(encoding.GetBytes(source), target);
        }

        /// <summary>
        /// Reads <see cref="byte[]"/> data of instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">
        /// Contains <see cref="Stream"/> value for reading process.
        /// </param>
        /// <param name="length">
        /// Contains <see cref="int"/> value with length of <see cref="string"/> data.
        /// </param>
        public static byte[] Read(Stream source, int length)
        {
            try
            {
                BinaryReader binaryReader = new BinaryReader(source);

                return binaryReader.ReadBytes(length);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Reads <see cref="string"/> data of instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">
        /// Contains <see cref="Stream"/> value for reading process.
        /// </param>
        /// <param name="length">
        /// Contains <see cref="int"/> value with length of <see cref="string"/> data.
        /// </param>
        /// <param name="encoding">
        /// Contains optional instance of <see cref="Encoding"/>.
        /// </param>
        public static string ReadString(Stream source, int length, Encoding encoding = null)
        {
            SetEncoding(ref encoding);

            return encoding.GetString(Read(source, length));
        }

        /// <summary>
        /// Resets instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="s">
        /// Contains instance of <see cref="Stream"/>.
        /// </param>
        public static void ResetStream(Stream s) => s.Seek(0, SeekOrigin.Begin);

        #endregion

        #region Private Functions

        /// <summary>
        /// Sets default instance of <see cref="Encoding"/>.
        /// </summary>
        /// <param name="encoding">
        /// Contains instance of <see cref="Encoding"/>.
        /// </param>
        private static void SetEncoding(ref Encoding encoding) => encoding = encoding ?? Encoding.UTF8;

        #endregion
    }
}
