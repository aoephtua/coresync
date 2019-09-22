// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CryptLib;
using CoreSync.CryptLib.Core;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public class CoreSyncFileEntry : CoreSyncVaultEntryBase<CoreSyncFileEntry>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public CoreSyncFileEntry() => GenerateFileDataPassphrase();

        #endregion

        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with directory name of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public const string DirectoryName = "f";

        /// <summary>
        /// Contains <see cref="string"/> value with data directory name of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public const string DataDirectoryName = "d";

        /// <summary>
        /// Contains <see cref="string"/> value with file hashing algorithm of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public const string FileHashingAlgorithm = "MD5";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Contains <see cref="string"/> value with relative file name of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        protected override string RelativeFileName => Path.Combine(DirectoryName, GetIdentifierAsFileName(identifier));

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains <see cref="Guid"/> with identifier of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        [DataMember(Name = "ID", Order = 0)]
        private Guid identifier = Guid.NewGuid();

        /// <summary>
        /// Gets or sets <see cref="Guid"/> with identifier of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public Guid Identifier
        {
            get => identifier;
            set => identifier = value;
        }

        /// <summary>
        /// Gets <see cref="string"/> value with <see cref="Guid"/> identifier as file name of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public string IdentifierAsFileName => GetIdentifierAsFileName(identifier);

        /// <summary>
        /// Contains <see cref="string"/> value with file data checksum of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        [DataMember(Name = "Checksum", Order = 1)]
        private string fileDataChecksum;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with file data checksum of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public string FileDataChecksum
        {
            get => fileDataChecksum;
            set => fileDataChecksum = value;
        }

        /// <summary>
        /// Contains <see cref="string"/> value with file data passphrase of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        [DataMember(Name = "Passphrase", Order = 2)]
        private string fileDataPassphrase;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with file data passphrase of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        public string FileDataPassphrase
        {
            get => fileDataPassphrase;
            set => fileDataPassphrase = value;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets and adds source instance of <see cref="CoreSyncFileEntry"/> to <see cref="CoreSyncRepository"/>.
        /// </summary>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with file name of <see cref="CoreSyncFileEntry"/>.
        /// </param>
        /// <returns>
        /// Returns instance <see cref="CoreSyncFileEntry"/>.
        /// </returns>
        public static CoreSyncFileEntry GetSourceFileEntry(string fileName)
        {
            CoreSyncFileEntry entry = null;

            var checksum = CalculateFileDataChecksum(fileName);

            if (!string.IsNullOrEmpty(checksum))
            {
                entry = CoreSyncRepository.SingletonInstance.FileEntries.FirstOrDefault(x => x.FileDataChecksum == checksum);

                if (entry == null)
                {
                    entry = new CoreSyncFileEntry() { FileDataChecksum = checksum };

                    CoreSyncRepository.SingletonInstance.FileEntries.Add(entry);
                }
            }

            return entry;
        }

        /// <summary>
        /// Decrypts instance of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        /// <param name="identifier">
        /// Contains <see cref="Guid"/> value with identifier.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncFileEntry"/>.
        /// </returns>
        public static CoreSyncFileEntry DecryptInstance(Guid identifier)
        {
            return DecryptInstance(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(DirectoryName, GetIdentifierAsFileName(identifier)), 
                CoreSyncMasterVault.SingletonInstance.FileEntryPassphrase);
        }

        /// <summary>
        /// Calculates <see cref="string"/> value with file data hash of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        /// <param name="filename">
        /// Contains <see cref="string"/> value with filename.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with file data checksum.
        /// </returns>
        public static string CalculateFileDataChecksum(string filename)
        {
            string checksum = null;

            try
            {
                using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    checksum = HashVault.GetHexStringFromBytes(HashVault.Compute(fs, FileHashingAlgorithm)).ToUpperInvariant();
                }
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return checksum;
        }

        /// <summary>
        /// Gets <see cref="Guid"/> value with identifier of <see cref="CoreSyncFileEntry"/> as <see cref="string"/> value.
        /// </summary>
        /// <param name="id">
        /// Contains <see cref="Guid"/> value with identifier.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with identifier.
        /// </returns>
        public static string GetIdentifierAsFileName(Guid identifier)
        {
            return CoreSyncRepository.SplitFilenameForParentDirectory(identifier.ToString("N").ToUpperInvariant());
        }

        /// <summary>
        /// Encrypts instance of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public bool EncryptInstance()
        {
            return base.EncryptInstance(CoreSyncMasterVault.SingletonInstance.FileEntryPassphrase);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Generates <see cref="string"/> value with file data passphrase of <see cref="CoreSyncFileEntry"/>.
        /// </summary>
        private void GenerateFileDataPassphrase() => FileDataPassphrase = SymmetricCoreCryptor.GeneratePassphrase();

        #endregion
    }
}
