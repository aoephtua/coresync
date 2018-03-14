// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CryptLib.Core;
using System.IO;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public class CoreSyncMasterVault : CoreSyncVaultEntryBase<CoreSyncMasterVault>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public CoreSyncMasterVault()
        {
            this.headEntryPassphrase = SymmetricCoreCryptor.GeneratePassphrase();
            this.fileEntryPassphrase = SymmetricCoreCryptor.GeneratePassphrase();

            this.headEntrySalt = SymmetricCoreCryptor.GetSalt();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with master vault file name of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public const string MasterVaultFileName = "csmaster";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets <see cref="string"/> value with relative file name of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        protected override string RelativeFileName => CoreSyncMasterVault.MasterVaultFileName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains singleton instance of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        private static CoreSyncMasterVault singletonInstance;

        /// <summary>
        /// Gets singleton instance of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public static CoreSyncMasterVault SingletonInstance
        {
            get
            {
                singletonInstance = singletonInstance ?? CoreSyncMasterVault.DecryptInstance(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncMasterVault.MasterVaultFileName), CoreSyncConfiguration.SingletonInstance.Passphrase) ?? new CoreSyncMasterVault();

                return singletonInstance;
            }
        }

        /// <summary>
        /// Contains <see cref="string"/> value with head entry passphrase of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        [DataMember(Name = "HeadEntryPassphrase", Order = 0)]
        private string headEntryPassphrase;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with head entry passphrase of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public string HeadEntryPassphrase
        {
            get
            {
                return this.headEntryPassphrase;
            }
            set
            {
                this.headEntryPassphrase = value;
            }
        }

        /// <summary>
        /// Contains <see cref="byte[]"/> value with head entry salt of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        [DataMember(Name = "HeadEntrySalt", Order = 1)]
        private byte[] headEntrySalt;

        /// <summary>
        /// Gets or sets <see cref="byte[]"/> value with head entry salt of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public byte[] HeadEntrySalt
        {
            get
            {
                return this.headEntrySalt;
            }
            set
            {
                this.headEntrySalt = value;
            }
        }

        /// <summary>
        /// Contains <see cref="string"/> value with file entry passphrase of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        [DataMember(Name = "FileEntryPassphrase", Order = 2)]
        private string fileEntryPassphrase;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with file entry passphrase of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        public string FileEntryPassphrase
        {
            get
            {
                return this.fileEntryPassphrase;
            }
            set
            {
                this.fileEntryPassphrase = value;
            }
        }

        /// <summary>
        /// Gets whether local instance of <see cref="CoreSyncMasterVault"/> exists.
        /// </summary>
        public static bool MasterVaultExists
        {
            get
            {
                return File.Exists(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncMasterVault.MasterVaultFileName));
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Encrypts instance of <see cref="CoreSyncMasterVault"/>.
        /// </summary>
        /// <param name="force">
        /// Contains whether overwriting is forced.
        /// </param>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public bool EncryptInstance(bool force = false)
        {
            return force || !CoreSyncMasterVault.MasterVaultExists ? base.EncryptInstance(CoreSyncConfiguration.SingletonInstance.Passphrase) : false;
        }

        #endregion
    }
}
