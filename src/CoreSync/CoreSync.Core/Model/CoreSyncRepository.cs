// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.IO;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

#endregion

namespace CoreSync.Core.Model
{
    [DataContract]
    public class CoreSyncRepository : CoreSyncEncryptableBase<CoreSyncRepository>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CoreSyncRepository"/>.
        /// </summary>
        public CoreSyncRepository()
        {
            this.ConfigureHeadEntriesCollection();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with file name of <see cref="CoreSyncRepository"/>.
        /// </summary>
        public const string FileName = "repository";

        /// <summary>
        /// Contains <see cref="int"/> value with character length of parent directory for splitted file name.
        /// </summary>
        public const int ParentDirectoryOfSplittedFileNameLength = 8;

        #endregion

        #region Private Members

        /// <summary>
        /// Contains <see cref="string"/> value with full name of <see cref="CoreSyncRepository"/>.
        /// </summary>
        private static string FullName = CoreSyncProcessor.GetFullName(CoreSyncRepository.FileName);

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets <see cref="string"/> value with target file name of <see cref="CoreSyncRepository"/>.
        /// </summary>
        protected override string TargetFileName => CoreSyncRepository.FullName;

        #endregion

        #region Public Properties

        /// <summary>
        /// Contains singleton instance of <see cref="CoreSyncRepository"/>.
        /// </summary>
        private static CoreSyncRepository singletonInstance;

        /// <summary>
        /// Gets singleton instance of <see cref="CoreSyncRepository"/>.
        /// </summary>
        public static CoreSyncRepository SingletonInstance
        {
            get
            {
#if DEBUG
                singletonInstance = singletonInstance ?? CoreSyncRepository.Deserialize() ?? new CoreSyncRepository();
#else
                singletonInstance = singletonInstance ?? CoreSyncRepository.DecryptInstance(CoreSyncRepository.FullName, CoreSyncConfiguration.SingletonInstance.Passphrase) ?? new CoreSyncRepository();
#endif            

                return singletonInstance;
            }
        }

        /// <summary>
        /// Contains instance of <see cref="ObservableCollection{CCloudHeadEntry}"/>.
        /// </summary>
        [DataMember(Order = 0, Name = "HeadEntries")]
        private ObservableCollection<CoreSyncHeadEntry> headEntries = new ObservableCollection<CoreSyncHeadEntry>();

        /// <summary>
        /// Gets instance of <see cref="ObservableCollection{CCloudHeadEntry}"/>.
        /// </summary>
        public ObservableCollection<CoreSyncHeadEntry> HeadEntries
        {
            get
            {
                return this.headEntries;
            }
        }

        /// <summary>
        /// Contains instance of <see cref="ObservableCollection{CCloudFileEntry}"/>.
        /// </summary>
        [DataMember(Order = 1, Name = "FileEntries")]
        private ObservableCollection<CoreSyncFileEntry> fileEntries = new ObservableCollection<CoreSyncFileEntry>();

        /// <summary>
        /// Gets instance of <see cref="ObservableCollection{CCloudFileEntry}"/>.
        /// </summary>
        public ObservableCollection<CoreSyncFileEntry> FileEntries
        {
            get
            {
                return this.fileEntries;
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Splits file name for parent directory entry of <see cref="CoreSyncRepository"/>.
        /// </summary>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with file name.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with splitted file name.
        /// </returns>
        public static string SplitFilenameForParentDirectory(string fileName)
        {
            if (fileName.Length > CoreSyncRepository.ParentDirectoryOfSplittedFileNameLength)
            {
                return fileName.Insert(CoreSyncRepository.ParentDirectoryOfSplittedFileNameLength, Path.DirectorySeparatorChar.ToString());
            }

            return fileName;
        }

        /// <summary>
        /// Saves instance of <see cref="CoreSyncRepository"/> to file system.
        /// </summary>
        public void SaveToFileSystem()
        {
            if (this.headEntries.Any())
            {
#if DEBUG
                base.SerializeToLocalFile(true);
#else
                base.EncryptInstance(CoreSyncConfiguration.SingletonInstance.Passphrase);      
#endif
            } 
        }

        /// <summary>
        /// Processes synchroniaztion of file system entries of <see cref="CoreSyncRepository"/>.
        /// </summary>
        /// <param name="configuration">
        /// Contains instance of <see cref="CoreSyncConfiguration"/>.
        /// </param>
        public void ProcessSynchronizationOfFileSystemEntries()
        {
            if (CoreSyncConfiguration.SingletonInstance != null)
            {
                var sourceHeadEntriesBeforeSync = this.headEntries.Any();

                var encryptedHeadEntries = DataProcessor.GetEntries(CoreSyncConfiguration.SingletonInstance.GetEncryptedDirectory(CoreSyncHeadEntry.DirectoryName), true);

                if (encryptedHeadEntries.Any())
                {
                    CoreSyncHeadEntry.DeleteSourceHeadEntries(encryptedHeadEntries);
                    CoreSyncHeadEntry.DecryptHeadEntries(encryptedHeadEntries);
                }

                var fileSystemEntryNames = Directory.GetFileSystemEntries(CoreSyncProcessor.WorkingDirectoryPath, "*", SearchOption.AllDirectories)
                    .Where(x => !x.Contains(CoreSyncProcessor.BaseDirectoryName) && CoreSyncConfiguration.IsValidEntryName(x))
                    .ToList();

                CoreSyncHeadEntry.EncryptHeadEntries(encryptedHeadEntries, fileSystemEntryNames);

                if (sourceHeadEntriesBeforeSync)
                {
                    CoreSyncHeadEntry.DeleteEncryptedHeadEntries(fileSystemEntryNames);
                }
            }
            else
            {
                CoreSyncProcessor.Log("Invalid configuration file", CoreSyncLogLevel.Error);
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Deserializes to instance of <see cref="CoreSyncRepository"/>.
        /// </summary>
        /// <returns>
        /// Returns instance of <see cref="CoreSyncRepository"/>.
        /// </returns>
        private static CoreSyncRepository Deserialize()
        {
            return CoreSyncSerializeableBase<CoreSyncRepository>.DeserializeFromLocalFile(CoreSyncRepository.FullName);
        }

        /// <summary>
        /// Method is called on deserialization of <see cref="CoreSyncRepository"/> instance.
        /// </summary>
        /// <param name="context">
        /// Contains instance of <see cref="StreamingContext"/>.
        /// </param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ConfigureHeadEntriesCollection();
        }

        /// <summary>
        /// Configures <see cref="ObservableCollection{CCloudHeadEntry}"/> instance of <see cref="CoreSyncRepository"/>.
        /// </summary>
        private void ConfigureHeadEntriesCollection()
        {
            this.headEntries.CollectionChanged += (s, e) =>
            {
                if (this.headEntries.Any())
                {
                    if (this.headEntries.Count > 1)
                    {
                        this.headEntries = new ObservableCollection<CoreSyncHeadEntry>(CoreSyncRepository.SingletonInstance.HeadEntries.OrderBy(x => x.RelativeName));

                        this.ConfigureHeadEntriesCollection();
                    }

                    this.SaveToFileSystem();
                }
                else
                {
                    CoreSyncMasterVault.SingletonInstance.DeleteInstance(true);

                    base.DeleteInstance(true);
                }
            };
        }

        #endregion
    }
}
