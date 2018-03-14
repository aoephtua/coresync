// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.Model;
using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace CoreSync.Core
{
    public static class CoreSyncProcessor
    {
        #region Public Members

        /// <summary>
        /// Contains <see cref="string"/> value with application name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public const string ApplicationName = "coresync";

        /// <summary>
        /// Contains <see cref="string"/> value with base directory name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public const string BaseDirectoryName = "." + CoreSyncProcessor.ApplicationName;

        /// <summary>
        /// Contains <see cref="string"/> value with log directory name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public const string LogDirectoryName = "logs";

        /// <summary>
        /// Cotains <see cref="string"/> value with full qualified path of working directory of <see cref="CoreSyncProcessor"/>.
        /// </summary>
#if DEBUG
        public static string WorkingDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), CoreSyncProcessor.ApplicationName);
#else
        public static string WorkingDirectoryPath = CoreSyncProcessor.GetDataDirectory(Environment.CurrentDirectory);
#endif

        /// <summary>
        /// Contains <see cref="LogDelegate"/> instance of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static LogDelegate LogNotification;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether current operating system equals <see cref="OSPlatform.Windows"/>.
        /// </summary>
        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        #endregion

        #region Public Functions

        /// <summary>
        /// Adds instance of <see cref="CoreSyncLogEntry"/> to <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="data">
        /// Contains <see cref="string"/> value with log data.
        /// </param>
        /// <param name="logLevel">
        /// Contains value of <see cref="CoreSyncLogLevel"/>.
        /// </param>
        /// <param name="writeLogEntry">
        /// Contains whether log entry should be written.
        /// </param>
        public static void Log(string data, CoreSyncLogLevel logLevel = CoreSyncLogLevel.Info, bool writeLogEntry = true)
        {
            if (!String.IsNullOrEmpty(data))
            {
                var logEntry = new CoreSyncLogEntry()
                {
                    LogLevel = logLevel,
                    Date = DateTime.Now,
                    Data = data,
                    DataOnly = !writeLogEntry
                };

                if (writeLogEntry)
                {
                    var logFileName = CoreSyncProcessor.GetFullName(Path.Combine(CoreSyncProcessor.LogDirectoryName, String.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd"))));

                    Directory.CreateDirectory(new FileInfo(logFileName).DirectoryName);

                    using (var logWriter = File.Exists(logFileName) ? File.AppendText(logFileName) : File.CreateText(logFileName))
                    {
                        logWriter.WriteLine(logEntry.ToString());
                    }
                }

                CoreSyncProcessor.LogNotification(logEntry);
            }
        }

        /// <summary>
        /// Adds instance of <see cref="CoreSyncLogEntry"/> to <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="e">
        /// Contains instance of <see cref="Exception"/>.
        /// </param>
        /// <param name="logLevel">
        /// Contains value of <see cref="CoreSyncLogLevel"/>.
        /// </param>
        /// <param name="writeLogEntry">
        /// Contains whether log entry should be written.
        /// </param>
        public static void Log(Exception e, CoreSyncLogLevel logLevel = CoreSyncLogLevel.Error, bool writeLogEntry = true)
        {
            CoreSyncProcessor.Log(e.ToString(), CoreSyncLogLevel.Error, writeLogEntry);
        }

        /// <summary>
        /// Gets <see cref="string"/> value with full data directory name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="directory">
        /// Contains <see cref="string"/> value with directory name.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with target directory.
        /// </returns>
        public static string GetDataDirectory(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);

            return directoryInfo.Name == CoreSyncProcessor.BaseDirectoryName ? directoryInfo.Parent.FullName : directory;
        }

        /// <summary>
        /// Gets <see cref="string"/> value with full base directory of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with directory path.
        /// </returns>
        public static string GetFullBaseDirectory()
        {
            return Path.Combine(CoreSyncProcessor.WorkingDirectoryPath, CoreSyncProcessor.BaseDirectoryName);
        }

        /// <summary>
        /// Gets <see cref="string"/> value with full name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="filename">
        /// Contains <see cref="string"/> value with filename.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with full name.
        /// </returns>
        public static string GetFullName(string filename)
        {
            return Path.Combine(CoreSyncProcessor.GetFullBaseDirectory(), filename);
        }

        /// <summary>
        /// Executes functionalities related to command for initializing of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <param name="encryptedDirectory">
        /// Contains <see cref="string"/> value with encrypted directory path.
        /// </param>
        public static void Initialize(string passphrase, string encryptedDirectory = null)
        {
            if (!String.IsNullOrEmpty(passphrase))
            {
                if (!CoreSyncConfiguration.FileExists)
                {
                    var directory = Directory.CreateDirectory(CoreSyncProcessor.GetFullBaseDirectory());

                    directory.Attributes |= FileAttributes.Hidden;

                    var configuration = new CoreSyncConfiguration(passphrase)
                    {
                        EncryptedDirectory = encryptedDirectory
                    };

                    configuration.SerializeToLocalFile(true);

                    CoreSyncProcessor.Log("Initialization completed successfully.", writeLogEntry: false);
                }
                else
                {
                    CoreSyncProcessor.Log("Configuration file already exists. Use 'config' command.", logLevel: CoreSyncLogLevel.Error, writeLogEntry: false);
                }
            }
        }

        /// <summary>
        /// Executes functionalities related to command for configuring of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="encryptedDirectory">
        /// Contains <see cref="string"/> value with encrypted directory path.
        /// </param>
        public static void Configure(string encryptedDirectory)
        {
            var config = CoreSyncConfiguration.SingletonInstance;

            if (config.SetEncryptedDirectory(encryptedDirectory))
            {
                config.SerializeToLocalFile(true);
            }
            else
            {
                CoreSyncProcessor.Log("Configuring requires valid values.", logLevel: CoreSyncLogLevel.Error, writeLogEntry: false);
            }
        }

        /// <summary>
        /// Executes functionalities related to command for setting passphrase of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        public static void SetPassphrase(string passphrase)
        {
            var config = CoreSyncConfiguration.SingletonInstance;

            if (config.SetPassphrase(passphrase))
            {
                config.SerializeToLocalFile(true);

                CoreSyncProcessor.Log("Passphrase changed successfully.", writeLogEntry: false);
            }
            else
            {
                CoreSyncProcessor.Log("Passphrase hasn't been changed.", logLevel: CoreSyncLogLevel.Error, writeLogEntry: false);
            }
        }

        /// <summary>
        /// Executes functionalities related to command for synchronization of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static void Synchronize()
        {
            CoreSyncRepository.SingletonInstance.ProcessSynchronizationOfFileSystemEntries();

            CoreSyncProcessor.Log("Synchronization completed.", writeLogEntry: false);
        }

        /// <summary>
        /// Executes functionalities related to command for detaching of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="directoryPath">
        /// Contains <see cref="string"/> value with optional directory path.
        /// </param>
        public static void Detach(string directoryPath = null)
        {
            directoryPath = directoryPath ?? CoreSyncProcessor.GetFullBaseDirectory();

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);

                CoreSyncProcessor.Log("Detaching completed successfully.", writeLogEntry: false);
            }
            else
            {
                CoreSyncProcessor.Log(String.Format("Directory \"{0}\" does not exists.", directoryPath), logLevel: CoreSyncLogLevel.Error, writeLogEntry: false);
            }
        }

        /// <summary>
        /// Executes functionalities related to command for resetting of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <param name="encryptedDirectory">
        /// Contains <see cref="string"/> value with encrypted directory path.
        /// </param>
        public static void Reset(string passphrase, string encryptedDirectory = null)
        {
            if (!String.IsNullOrEmpty(passphrase))
            {
                var directoryPath = CoreSyncProcessor.GetFullBaseDirectory();

                if (Directory.Exists(directoryPath))
                {
                    CoreSyncProcessor.Detach(directoryPath);
                    CoreSyncProcessor.Initialize(passphrase, encryptedDirectory);
                }
                else
                {
                    CoreSyncProcessor.Log(String.Format("Directory \"{0}\" does not exists.", directoryPath), logLevel: CoreSyncLogLevel.Error, writeLogEntry: false);
                }
            }
        }

        /// <summary>
        /// Gets <see cref="string"/> value with current folder path of <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with full directory.
        /// </returns>
        public static string GetApplicationDataFolderPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CoreSyncProcessor.ApplicationName);
        }

        /// <summary>
        /// Creates application data <see cref="Directory"/> of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static void CreateApplicationDataFolder()
        {
            Directory.CreateDirectory(CoreSyncProcessor.GetApplicationDataFolderPath());
        }

        #endregion
    }
}
