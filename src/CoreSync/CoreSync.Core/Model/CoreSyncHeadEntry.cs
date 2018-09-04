// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.IO;
using CoreSync.CryptLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public class CoreSyncHeadEntry : CoreSyncVaultEntryBase<CoreSyncHeadEntry>
    {
        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with directory name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public const string DirectoryName = "h";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets <see cref="string"/> value with relative file name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        protected override string RelativeFileName => Path.Combine(DirectoryName, ProcessEncryptedHeadFilename());

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains <see cref="string"/> value with relative name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        [DataMember(Name = "Name", Order = 0)]
        private string relativeName;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with relative name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public string RelativeName
        {
            get => relativeName;
            set
            {
                if (relativeName != value)
                {
                    relativeName = value;

                    if (IsFile = File.Exists(FullName))
                    {
                        FileEntry = CoreSyncFileEntry.GetSourceFileEntry(FullName);

                        if (FileEntry != null)
                        {
                            FileEntryIdentifier = FileEntry.Identifier;
                        }
                    }

                    FileName = ProcessEncryptedHeadFilename();
                }
            }
        }

        /// <summary>
        /// Gets <see cref="string"/> value with full name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public string FullName => Path.Combine(CoreSyncProcessor.WorkingDirectoryPath, relativeName);

        /// <summary>
        /// Contains <see cref="string"/> value with head file name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        [DataMember(Name = "FileName", Order = 1)]
        private string fileName;

        /// <summary>
        /// Gets or sets <see cref="string"/> value with head file name of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        /// <summary>
        /// Contains <see cref="Guid"/> value with <see cref="CoreSyncFileEntry"/> identifier of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        [DataMember(Name = "FileID", Order = 2, EmitDefaultValue = false)]
        private Guid fileEntryIdentifier;

        /// <summary>
        /// Gets or sets <see cref="Guid"/> value with <see cref="CoreSyncFileEntry"/> identifier of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public Guid FileEntryIdentifier
        {
            get => fileEntryIdentifier;
            set => fileEntryIdentifier = value;
        }

        /// <summary>
        /// Contains <see cref="CoreSyncFileEntry"/> instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        private CoreSyncFileEntry fileEntry;

        /// <summary>
        /// Gets or sets <see cref="CoreSyncFileEntry"/> instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        public CoreSyncFileEntry FileEntry
        {
            get
            {
                if (fileEntry == null && IsFile)
                {
                    fileEntry = CoreSyncRepository.SingletonInstance.FileEntries.FirstOrDefault(x => x.Identifier == FileEntryIdentifier);
                }

                return fileEntry;
            }
            set => fileEntry = value;
        }

        /// <summary>
        /// Gets or sets whether instance of <see cref="CoreSyncHeadEntry"/> is related to <see cref="File"/>.
        /// </summary>
        public bool IsFile { get; private set; }

        /// <summary>
        /// Gets whether instance of <see cref="CoreSyncHeadEntry"/> is valid.
        /// </summary>
        public bool IsValid => (IsFile && FileEntry != null) || (!IsFile && !Directory.EnumerateFileSystemEntries(FullName).Any());

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets and adds source instance of <see cref="CoreSyncHeadEntry"/> to <see cref="CoreSyncRepository"/>.
        /// </summary>
        /// <param name="fileSystemEntryName">
        /// Contains <see cref="string"/> value with full name of file system entry.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncHeadEntry"/>.
        /// </returns>
        public static CoreSyncHeadEntry GetSourceHeadEntry(string fileSystemEntryName)
        {
            var sourceHeadEntry = new CoreSyncHeadEntry()
            {
                RelativeName = fileSystemEntryName.Substring(CoreSyncProcessor.WorkingDirectoryPath.Length + 1)
            };

            if (CoreSyncRepository.SingletonInstance.HeadEntries.FirstOrDefault(x => x.FileName == sourceHeadEntry.FileName) is CoreSyncHeadEntry existingEntry)
            {
                return existingEntry;
            }
            else
            {
                if (sourceHeadEntry.IsValid)
                {
                    CoreSyncRepository.SingletonInstance.HeadEntries.Add(sourceHeadEntry);

                    return sourceHeadEntry;
                }
                else if (sourceHeadEntry.IsFile)
                {
                    CoreSyncProcessor.Log(string.Format("{0} Invalid entry \"{1}\".", DateTime.Now.ToString(), fileSystemEntryName));
                }
            }

            return null;
        }

        /// <summary>
        /// Gets decrypted instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with file name of <see cref="CoreSyncHeadEntry"/>.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncHeadEntry"/>.
        /// </returns>
        public static CoreSyncHeadEntry GetDecryptedHeadEntry(string fileName)
        {
            var decryptedHeadEntry = DecryptInstance(fileName);

            if (decryptedHeadEntry != null && decryptedHeadEntry.IsFile)
            {
                decryptedHeadEntry.FileEntry = CoreSyncFileEntry.DecryptInstance(decryptedHeadEntry.FileEntryIdentifier);
            }

            return decryptedHeadEntry;
        }

        /// <summary>
        /// Deletes source instances of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="encryptedHeadEntries">
        /// Contains instance of <see cref="List{Tuple{string, string}}"/> with encrypted head entries.
        /// </param>
        public static void DeleteSourceHeadEntries(List<Tuple<string, string>> encryptedHeadEntries)
        {
            CoreSyncProcessor.Log("Initializing deletion of file system entries.", writeLogEntry: false);

            var headEntriesForDeletion = CoreSyncRepository.SingletonInstance.HeadEntries.Where(x => !encryptedHeadEntries.Any(x2 => x2.Item2 == x.FileName)).ToList();

            if (headEntriesForDeletion.Any())
            {
                foreach (var headEntryForDeletion in headEntriesForDeletion)
                {
                    if (headEntryForDeletion.DeleteSourceInstance())
                    {
                        CoreSyncProcessor.Log(string.Format("Deleted entry \"{0}\".", headEntryForDeletion.FullName));
                    }
                }
            }
            else
            {
                CoreSyncProcessor.Log("No source entries for deletion found.", writeLogEntry: false);
            }
        }

        /// <summary>
        /// Decrypts instances of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="encryptedHeadEntries">
        /// Contains instance of <see cref="List{Tuple{string, string}}"/> with encrypted head entries.
        /// </param>
        public static void DecryptHeadEntries(List<Tuple<string, string>> encryptedHeadEntries)
        {
            CoreSyncProcessor.Log("Initializing vault entries decryption.", writeLogEntry: false);

            var encryptedHeadEntriesNames = encryptedHeadEntries.Where(x => !CoreSyncRepository.SingletonInstance.HeadEntries.Any(x2 => x2.FileName == x.Item2));

            if (encryptedHeadEntriesNames.Any())
            {
                foreach (var encryptedHeadEntryName in encryptedHeadEntriesNames)
                {
                    var fileName = Path.Combine(encryptedHeadEntryName.Item1, encryptedHeadEntryName.Item2);

                    if (GetDecryptedHeadEntry(fileName) is CoreSyncHeadEntry headEntry)
                    {
                        if (headEntry.IsFile)
                        {
                            if (headEntry.IsValid)
                            {
                                var encryptedDirectory = CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncFileEntry.DataDirectoryName, 
                                    CoreSyncRepository.SplitFilenameForParentDirectory(headEntry.FileEntry.FileDataChecksum));

                                DataProcessor.Decrypt(headEntry.FullName, encryptedDirectory, headEntry.FileEntry.FileDataPassphrase);

                                if (!CoreSyncRepository.SingletonInstance.FileEntries.Any(x => x.FileDataChecksum == headEntry.FileEntry.FileDataChecksum))
                                {
                                    CoreSyncRepository.SingletonInstance.FileEntries.Add(headEntry.FileEntry);
                                }
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(headEntry.FullName);
                        }

                        CoreSyncRepository.SingletonInstance.HeadEntries.Add(headEntry);

                        CoreSyncProcessor.Log(string.Format("Decrypted entry \"{0}\".", headEntry.FullName));
                    }
                }
            }
            else
            {
                CoreSyncProcessor.Log("No vault entries for decryption found.", writeLogEntry: false);
            }
        }

        /// <summary>
        /// Encrypts instances of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="encryptedHeadEntries">
        /// Contains instance of <see cref="List{Tuple{string, string}}"/> with encrypted head entries.
        /// </param>
        /// <param name="fileSystemEntryNames">
        /// Contains <see cref="List{string}"/> with entries of file system.
        /// </param>
        public static void EncryptHeadEntries(List<Tuple<string, string>> encryptedHeadEntries, List<string> fileSystemEntryNames)
        {
            if (fileSystemEntryNames.Any())
            {
                CoreSyncProcessor.Log("Initializing file system entries encryption.", writeLogEntry: false);

                var decryptedFileEntries = DataProcessor.GetEntries(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncFileEntry.DirectoryName), true);

                var cnt = 0;

                foreach (var fileSystemEntryName in fileSystemEntryNames)
                {
                    if (GetSourceHeadEntry(fileSystemEntryName) is CoreSyncHeadEntry headEntry)
                    {
                        if (encryptedHeadEntries.FirstOrDefault(x => x.Item2 == headEntry.FileName) == null)
                        {
                            if (CoreSyncRepository.SingletonInstance.HeadEntries
                                .FirstOrDefault(x => x.RelativeName == headEntry.RelativeName && x.FileName != headEntry.FileName) is CoreSyncHeadEntry obsoleteEntry)
                            {
                                obsoleteEntry.DeleteInstance();
                            }

                            if (headEntry.IsFile && decryptedFileEntries.FirstOrDefault(x => x.Item2 == headEntry.FileEntry.IdentifierAsFileName) == null)
                            {
                                var encryptedDirectory = CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncFileEntry.DataDirectoryName, 
                                    CoreSyncRepository.SplitFilenameForParentDirectory(headEntry.FileEntry.FileDataChecksum));

                                DataProcessor.Encrypt(headEntry.FullName, encryptedDirectory, headEntry.FileEntry.FileDataPassphrase);

                                headEntry.FileEntry.EncryptInstance();
                            }

                            if (headEntry.EncryptInstance())
                            {
                                cnt++;

                                CoreSyncProcessor.Log(string.Format("Encrypted entry \"{0}\".", headEntry.FullName));
                            }
                        }
                    }
                }

                if (cnt == 0)
                {
                    CoreSyncProcessor.Log("No file system entries for encryption found.", writeLogEntry: false);
                }
            }
        }

        /// <summary>
        /// Deletes encrypted instances of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="fileSystemEntryNames">
        /// Contains <see cref="List{string}"/> with entries of file system.
        /// </param>
        public static void DeleteEncryptedHeadEntries(List<string> fileSystemEntryNames)
        {
            CoreSyncProcessor.Log("Initializing deletion of vault entries.", writeLogEntry: false);

            var removedHeadEntries = CoreSyncRepository.SingletonInstance.HeadEntries
                .Where(x => !fileSystemEntryNames.Any(x2 => x2 == x.FullName))
                .ToList();

            var needlessHeadEntries = CoreSyncRepository.SingletonInstance.HeadEntries
                .Where(x => !x.IsFile && CoreSyncRepository.SingletonInstance.HeadEntries
                    .Where(x2 => !removedHeadEntries.Any(x3 => x3.FullName == x2.FullName))
                    .Any(x2 => x.FullName != x2.FullName && x2.FullName.StartsWith(x.FullName + Path.DirectorySeparatorChar)))
                .ToList();

            var headEntriesForDeletion = removedHeadEntries.Concat(needlessHeadEntries);

            foreach (var sourceDeletedHeadEntry in headEntriesForDeletion)
            {
                sourceDeletedHeadEntry.DeleteInstance();
            }

            foreach (var sourceDeletedFileEntry in CoreSyncRepository.SingletonInstance.FileEntries
                .Where(x => !CoreSyncRepository.SingletonInstance.HeadEntries.Any(x2 => x2.FileEntryIdentifier == x.Identifier)).ToList())
            {
                if (sourceDeletedFileEntry.DeleteInstance(true))
                {
                    DataProcessor.Delete(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncFileEntry.DataDirectoryName,
                        CoreSyncRepository.SplitFilenameForParentDirectory(sourceDeletedFileEntry.FileDataChecksum)), true);

                    if (CoreSyncRepository.SingletonInstance.FileEntries.Remove(sourceDeletedFileEntry))
                    {
                        CoreSyncRepository.SingletonInstance.SaveToFileSystem();

                        CoreSyncProcessor.Log(string.Format("Deleted file with checksum {0}.", sourceDeletedFileEntry.FileDataChecksum));
                    }
                }
            }

            if (!headEntriesForDeletion.Any())
            {
                CoreSyncProcessor.Log("No vault entries for deletion found.", writeLogEntry: false);
            }
        }

        /// <summary>
        /// Decrypts instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with file name.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncHeadEntry"/>.
        /// </returns>
        public static CoreSyncHeadEntry DecryptInstance(string fileName) => 
            DecryptInstance(fileName, CoreSyncMasterVault.SingletonInstance.HeadEntryPassphrase);

        /// <summary>
        /// Deletes source instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <returns>
        /// Returns whether deletion process succeeded.
        /// </returns>
        public bool DeleteInstance()
        {
            if (base.DeleteInstance(true))
            {
                if (CoreSyncRepository.SingletonInstance.HeadEntries.Remove(this))
                {
                    CoreSyncProcessor.Log(string.Format("Deleted entry \"{0}\".", FullName));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Encrypts instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public bool EncryptInstance()
        {
            CoreSyncMasterVault.SingletonInstance.EncryptInstance();

            return base.EncryptInstance(CoreSyncMasterVault.SingletonInstance.HeadEntryPassphrase);
        }

        /// <summary>
        /// Deletes decrypted references of <see cref="CoreSyncHeadEntry"/> instance.
        /// </summary>
        /// <returns>
        /// Returns whether deletion process succeeded.
        /// </returns>
        public bool DeleteSourceInstance()
        {
            try
            {
                if (IsFile)
                {
                    File.Delete(FullName);

                    if (!CoreSyncRepository.SingletonInstance.HeadEntries.Any(x => x.TargetFileName != TargetFileName && x.FileEntryIdentifier == FileEntryIdentifier))
                    {
                        CoreSyncRepository.SingletonInstance.FileEntries.Remove(FileEntry);
                    }
                }
                else
                {
                    Directory.Delete(FullName);
                }

                CoreSyncRepository.SingletonInstance.HeadEntries.Remove(this);

                if (!IsFile)
                {
                    foreach (var headSubEntry in CoreSyncRepository.SingletonInstance.HeadEntries.Where(x => x.RelativeName.StartsWith(RelativeName)).ToList())
                    {
                        headSubEntry.DeleteSourceInstance();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return false;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Method is called on deserialization of <see cref="CoreSyncHeadEntry"/> instance.
        /// </summary>
        /// <param name="context">
        /// Contains instance of <see cref="StreamingContext"/>.
        /// </param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => IsFile = FileEntryIdentifier != Guid.Empty;

        /// <summary>
        /// Processes <see cref="string"/> value with filename for encrypted instance of <see cref="CoreSyncHeadEntry"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with filename.
        /// </returns>
        private string ProcessEncryptedHeadFilename()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                var value = relativeName + FileEntry?.FileDataChecksum;

                var hexString = HashVault.GetHexStringFromBytes(HashVault.Compute(value, CoreSyncMasterVault.SingletonInstance.HeadEntrySalt));

                return CoreSyncRepository.SplitFilenameForParentDirectory(hexString.ToUpperInvariant());
            }

            return FileName;
        }

        #endregion
    }
}
