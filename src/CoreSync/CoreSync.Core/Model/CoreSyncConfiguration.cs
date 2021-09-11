// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.IO;
using CoreSync.CryptLib;
using CoreSync.CryptLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public class CoreSyncConfiguration : CoreSyncSerializeableBase<CoreSyncConfiguration>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public CoreSyncConfiguration() { }

        /// <summary>
        /// Initializes a new instance of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        public CoreSyncConfiguration(string passphrase)
        {
            SetPassphrase(passphrase);

            filters = new List<string>() { "^(?!.*desktop.ini$).*", @"^(?!.*(\.db)$).*", @"^(?!.*(\\|/|^)(\.*)~).*$" };
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with file name of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public const string FileName = "config";

        /// <summary>
        /// Contains <see cref="string"/> value with default vault base directory of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public const string VaultBaseDirectory = "vault";

        #endregion

        #region Private Members

        /// <summary>
        /// Contains <see cref="string"/> value with full name of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        private static readonly string FullName = CoreSyncProcessor.GetFullName(FileName);

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets <see cref="string"/> value with target file name of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        protected override string TargetFileName => FullName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains singleton instance of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        private static CoreSyncConfiguration singletonInstance;

        /// <summary>
        /// Gets singleton instance of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public static CoreSyncConfiguration SingletonInstance
        {
            get
            {
                singletonInstance ??= Deserialize() ?? new CoreSyncConfiguration();

                return singletonInstance;
            }
        }

        /// <summary>
        /// Gets whether data file of <see cref="CoreSyncConfiguration"/> exists.
        /// </summary>
        public static bool FileExists => File.Exists(CoreSyncProcessor.GetFullName(FileName));

        /// <summary>
        /// Contains <see cref="Guid"/> value with unique identifier of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        [DataMember(Name = "Identifier", Order = 0, EmitDefaultValue = false)]
        private Guid identifier;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with unique identifier of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public Guid Identifier
        {
            get => identifier;
            private set
            {
                if (identifier == Guid.Empty)
                {
                    identifier = value;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="string"/> value with file name of identifier of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public string IdentifierFileName => Path.Combine(CoreSyncProcessor.GetApplicationDataFolderPath(), identifier.ToString("N"));

        /// <summary>
        /// Gets or sets <see cref="string"/> value with passphrase of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public string Passphrase { get; private set; }

        /// <summary>
        /// Contains <see cref="string"/> value with protected passphrase of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        [DataMember(Name = "Passphrase", Order = 1)]
        private string protectedPassphrase;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with protected passphrase of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public string ProtectedPassphrase
        {
            get => protectedPassphrase;
            private set => protectedPassphrase = value;
        }

        /// <summary>
        /// Contains <see cref="string"/> value with initialization vector of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        [DataMember(Name = "Entropy", Order = 2, EmitDefaultValue = false)]
        private string entropy;

        /// <summary>
        /// Gets or sets <see cref="byte[]"/> value with initialization vector of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public string Entropy
        {
            get => entropy;
            private set => entropy = value;
        }

        /// <summary>
        /// Contains <see cref="string"/> value with encrypted directory of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        [DataMember(Name = "EncryptedDirectory", Order = 3, EmitDefaultValue = false)]
        private string encryptedDirectory;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with encrypted directory of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public string EncryptedDirectory
        {
            get => encryptedDirectory;
            set => encryptedDirectory = value;
        }

        /// <summary>
        /// Contains <see cref="List{string}"/> with filters of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        [DataMember(Name = "Filters", Order = 4, EmitDefaultValue = false)]
        public List<string> filters;

        /// <summary>
        /// Gets or sets <see cref="List{string}"/> with filters of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        public List<string> Filters
        {
            get => filters;
            set => filters = value;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Checks whether relative entry name matches any filter of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <param name="name">
        /// Contains <see cref="string"/> value with full name.
        /// </param>
        /// <returns>
        /// Returns whether <see cref="string"/> value is valid.
        /// </returns>
        public static bool IsValidEntryName(string fullName)
        {
            if (!string.IsNullOrEmpty(fullName))
            {
                var relativeName = fullName.Replace(CoreSyncProcessor.WorkingDirectoryPath, string.Empty);

                foreach (var filter in SingletonInstance.Filters)
                {
                    if (!Regex.IsMatch(relativeName, filter))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Sets passphrase of <see cref="CoreSyncConfiguration"/> instance.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns whether passphrase has changed.
        /// </returns>
        public bool SetPassphrase(string passphrase)
        {
            if (!string.IsNullOrEmpty(passphrase) && Passphrase != passphrase)
            {
                CoreSyncMasterVault masterVault = CoreSyncMasterVault.MasterVaultExists ? CoreSyncMasterVault.SingletonInstance : null;

                var entropy = SymmetricCoreCryptor.GetSalt(32);

                Passphrase = passphrase;
                this.entropy = Convert.ToBase64String(entropy);

                SetProtectedKey(entropy);

                if (masterVault != null)
                {
                    masterVault.EncryptInstance(true);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets encrypted directory of <see cref="CoreSyncConfiguration"/> instance.
        /// </summary>
        /// <param name="encryptedDirectory">
        /// Contains <see cref="string"/> value with encrypted directory.
        /// </param>
        /// <returns>
        /// Returns whether encrypted directory has changed.
        /// </returns>
        public bool SetEncryptedDirectory(string encryptedDirectory)
        {
            if (!string.IsNullOrEmpty(encryptedDirectory) && this.encryptedDirectory != encryptedDirectory && Path.IsPathRooted(encryptedDirectory))
            {
                this.encryptedDirectory = encryptedDirectory;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets <see cref="string"/> value with encrypted directory of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <param name="paths">
        /// Contains <see cref="string[]"/> with paths of the path.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with target directory.
        /// </returns>
        public string GetEncryptedDirectory(params string[] paths)
        {
            if (!string.IsNullOrEmpty(encryptedDirectory))
            {
                return DataProcessor.Combine(encryptedDirectory, paths);
            }

            return DataProcessor.Combine(Path.Combine(CoreSyncProcessor.GetFullBaseDirectory(), VaultBaseDirectory), paths);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Deserializes to instance of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncConfiguration"/>.
        /// </returns>
        private static CoreSyncConfiguration Deserialize()
        {
            return DeserializeFromLocalFile(FullName);
        }

        /// <summary>
        /// Method is called on deserialization of <see cref="CoreSyncConfiguration"/> instance.
        /// </summary>
        /// <param name="context">
        /// Contains instance of <see cref="StreamingContext"/>.
        /// </param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (!string.IsNullOrEmpty(entropy))
            {
                SetPlainTextKey(Convert.FromBase64String(entropy));
            }
        }

        /// <summary>
        /// Sets encrypted <see cref="byte[]"/> with key of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <param name="entropy">
        /// Contains <see cref="byte[]"/> with entropy.
        /// </param>
        private void SetProtectedKey(byte[] entropy)
        {
            if (CoreSyncProcessor.IsWindows)
            {
                protectedPassphrase = Convert.ToBase64String(DPAVault.Protect(Passphrase, entropy));
            }
            else
            {
                Identifier = Guid.NewGuid();

                protectedPassphrase = SymmetricCoreCryptor.GeneratePassphrase(64);

                CoreSyncProcessor.CreateApplicationDataFolder();

                File.WriteAllBytes(IdentifierFileName, new RijndaelManagedCoreCryptor().Encrypt(Passphrase, protectedPassphrase, entropy));
            }
        }

        /// <summary>
        /// Sets <see cref="string"/> value with plain text key of <see cref="CoreSyncConfiguration"/>.
        /// </summary>
        /// <param name="entropy">
        /// Contains <see cref="byte[]"/> with entropy.
        /// </param>
        private void SetPlainTextKey(byte[] entropy)
        {
            if (CoreSyncProcessor.IsWindows)
            {
                Passphrase = DPAVault.Unprotect(Convert.FromBase64String(protectedPassphrase), entropy);
            }
            else
            {
                Passphrase = new RijndaelManagedCoreCryptor().Decrypt(File.ReadAllBytes(IdentifierFileName), protectedPassphrase, entropy);
            }
        }

        #endregion
    }
}
