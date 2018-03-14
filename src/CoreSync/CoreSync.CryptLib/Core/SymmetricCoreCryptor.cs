// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace CoreSync.CryptLib.Core
{
    public abstract class SymmetricCoreCryptor : ICryptor
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="SymmetricCoreCryptor"/> with default <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </summary>
        public SymmetricCoreCryptor() : this(new SymmetricCoreCryptorConfiguration()) { }

        /// <summary>
        /// Initializes a new instance of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        /// <param name="configuration">
        /// Contains instance of <see cref="SymmetricCoreCryptorConfiguration"/>.
        /// </param>
        public SymmetricCoreCryptor(SymmetricCoreCryptorConfiguration configuration)
        {
            this.Configuration = configuration;

            this.OnProgressChanged += delegate { };
            this.OnComplete += delegate { };
            this.OnError += delegate { };
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Contains buffer length of file transmitting of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        private static byte[] buffer = new byte[1024 * 1024];

        /// <summary>
        /// Contains flag for canceled status of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        private bool cancelFlag;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or set the <see cref="SymmetricCoreCryptorConfiguration"/> of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        public SymmetricCoreCryptorConfiguration Configuration { get; set; }

        /// <summary>
        /// Contains event to throw data for changes of file transmitting of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        public event ProgressChangeDelegate OnProgressChanged;

        /// <summary>
        /// Contains event to throw data for completed file transmitting of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        public event CompleteDelegate OnComplete;

        /// <summary>
        /// Contains event to throw error message of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        public event ErrorDelegate OnError;

        #endregion

        #region Public Functions

        /// <summary>
        /// Encryptes <see cref="string"/> value with <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="string"/> value with data for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="encoding">
        /// Contains instance of <see cref="Encoding"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> value with encrypted data.
        /// </returns>
        public virtual byte[] Encrypt(string data, string passphrase, byte[] salt, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            try
            {
                using (SymmetricAlgorithm SACrypto = this.CreateSymmetricAlgorithm(passphrase, salt))
                {
                    using (ICryptoTransform encryptor = SACrypto.CreateEncryptor(SACrypto.Key, SACrypto.IV))
                    {
                        byte[] plainBytes = encoding.GetBytes(data);

                        return encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    }
                }
            }
            catch (Exception e)
            {
                this.OnError(e);

                return null;
            }
        }

        /// <summary>
        /// Encryptes <see cref="Stream"/> value with <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        /// <param name="source">
        /// Contains source <see cref="Stream"/> for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/> for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for encryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        public virtual void Encrypt(Stream source, Stream target, string passphrase, byte[] salt)
        {
            try
            {
                using (SymmetricAlgorithm SACrypto = this.CreateSymmetricAlgorithm(passphrase, salt))
                {
                    using (ICryptoTransform encryptor = SACrypto.CreateEncryptor(SACrypto.Key, SACrypto.IV))
                    {
                        CryptoStream cs = new CryptoStream(target, encryptor, CryptoStreamMode.Write);

                        this.TransmitStreamData(source, cs);

                        cs.FlushFinalBlock();
                    }
                }
            }
            catch (Exception e)
            {
                this.OnError(e);

                return;
            }

            this.OnComplete();
        }

        /// <summary>
        /// Decryptes <see cref="string"/> value with <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="byte[]"/> value with encrypted data for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="encoding">
        /// Contains instance of <see cref="Encoding"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with decrypted data.
        /// </returns>
        public virtual string Decrypt(byte[] data, string passphrase, byte[] salt, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            try
            {
                using (SymmetricAlgorithm SACrypto = this.CreateSymmetricAlgorithm(passphrase, salt))
                {
                    using (ICryptoTransform decryptor = SACrypto.CreateDecryptor(SACrypto.Key, SACrypto.IV))
                    {
                        byte[] plainBytes = decryptor.TransformFinalBlock(data, 0, data.Length);

                        return encoding.GetString(plainBytes);
                    }
                }
            }
            catch (Exception e)
            {
                this.OnError(e);

                return null;
            }
        }

        /// <summary>
        /// Decryptes <see cref="string"/> value with <see cref="SymmetricAlgorithm"/>.
        /// </summary>
        /// <param name="source">
        /// Contains source <see cref="Stream"/> for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="target">
        /// Contains target <see cref="Stream"/> for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with salt for decryption with <see cref="SymmetricAlgorithm"/>.
        /// </param>
        public virtual void Decrypt(Stream source, Stream target, string passphrase, byte[] salt)
        {
            try
            {
                using (SymmetricAlgorithm SACrypto = this.CreateSymmetricAlgorithm(passphrase, salt))
                {
                    using (ICryptoTransform decryptor = SACrypto.CreateDecryptor(SACrypto.Key, SACrypto.IV))
                    {
                        CryptoStream cs = new CryptoStream(source, decryptor, CryptoStreamMode.Read);

                        this.TransmitStreamData(cs, target);
                    }
                }
            }
            catch (Exception e)
            {
                this.OnError(e);
            }

            this.OnComplete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GeneratePassphrase(int length = 32)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenBuffer = new byte[length];

                rng.GetBytes(tokenBuffer);

                return Convert.ToBase64String(tokenBuffer);
            }
        }

        /// <summary>
        /// Generates <see cref="byte[]"/> with IV of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        /// <param name="maximumSaltLength">
        /// Contains <see cref="int"/> value with maximum IV length.
        /// </param>
        /// <returns>
        /// Returns <see cref="byte[]"/> with IV.
        /// </returns>
        public static byte[] GetSalt(int maximumSaltLength = 32)
        {
            var salt = new byte[maximumSaltLength];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Transmits <see cref="Stream"/> data of <see cref="SymmetricCoreCryptor"/>.
        /// </summary>
        /// <param name="input">
        /// Contains input instance of <see cref="Stream"/>.
        /// </param>
        /// <param name="ouput">
        /// Contains output instance of <see cref="Stream"/>.
        /// </param>
        private void TransmitStreamData(Stream input, Stream ouput)
        {
            long totalBytes = 0;
            int currentBlockSize = 0;

            while ((currentBlockSize = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalBytes += currentBlockSize;

                ouput.Write(buffer, 0, currentBlockSize);

                this.cancelFlag = false;
                this.OnProgressChanged(totalBytes, ref this.cancelFlag);

                if (cancelFlag == true)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Creates instance of <see cref="SymmetricAlgorithm"/> by encryption key and salt.
        /// </summary>
        /// <param name="key">
        /// Contains <see cref="string"/> value with encryption key of <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <param name="salt">
        /// Contains <see cref="byte[]"/> value with encryption salt of <see cref="SymmetricAlgorithm"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="SymmetricAlgorithm"/> containing encryption values.
        /// </returns>
        protected abstract SymmetricAlgorithm CreateSymmetricAlgorithm(string key, byte[] salt);

        #endregion
    }
}
