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
        public const string BaseDirectoryName = "." + ApplicationName;

        /// <summary>
        /// Contains <see cref="string"/> value with log directory name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public const string LogDirectoryName = "logs";

        /// <summary>
        /// Cotains <see cref="string"/> value with full qualified path of working directory of <see cref="CoreSyncProcessor"/>.
        /// </summary>
#if DEBUG
        public static string WorkingDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ApplicationName);
#else
        public static string WorkingDirectoryPath = GetDataDirectory(Environment.CurrentDirectory);
#endif

        /// <summary>
        /// Contains <see cref="LogDelegate"/> instance of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static LogDelegate LogNotification;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string"/> value with subdirectory path of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static string SubdirectoryPath { get; set; }

        /// <summary>
        /// Gets <see cref="string"/> value with parent directory path of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static string ParentDirectoryPath => SubdirectoryPath ?? WorkingDirectoryPath;

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
            if (!string.IsNullOrEmpty(data))
            {
                var logEntry = new CoreSyncLogEntry()
                {
                    LogLevel = logLevel,
                    Date = DateTime.Now,
                    Data = data
                };

                if (writeLogEntry)
                {
                    var logFileName = GetFullName(Path.Combine(LogDirectoryName, string.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd"))));

                    Directory.CreateDirectory(new FileInfo(logFileName).DirectoryName);

                    using (var logWriter = File.Exists(logFileName) ? File.AppendText(logFileName) : File.CreateText(logFileName))
                    {
                        logWriter.WriteLine(logEntry.ToString());
                    }
                }

                LogNotification(logEntry);
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
        public static void Log(Exception e, CoreSyncLogLevel logLevel = CoreSyncLogLevel.Error, bool writeLogEntry = true) => 
            Log(e.ToString(), logLevel, writeLogEntry);

        /// <summary>
        /// Adds instance of <see cref="CoreSyncLogEntry"/> with error to <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="msg">
        /// Contains <see cref="string"/> with error message.
        /// </param>
        /// <param name="writeLogEntry">
        /// Contains whether log entry should be written.
        /// </param>
        public static void Error(string msg, bool writeLogEntry = true) => Log(msg, CoreSyncLogLevel.Error, writeLogEntry);

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

            return directoryInfo.Name == BaseDirectoryName ? directoryInfo.Parent.FullName : directory;
        }

        /// <summary>
        /// Gets <see cref="string"/> value with full base directory of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with directory path.
        /// </returns>
        public static string GetFullBaseDirectory() => Path.Combine(WorkingDirectoryPath, BaseDirectoryName);

        /// <summary>
        /// Gets <see cref="string"/> value with full name of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="filename">
        /// Contains <see cref="string"/> value with filename.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with full name.
        /// </returns>
        public static string GetFullName(string filename) => Path.Combine(GetFullBaseDirectory(), filename);

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
            if (!string.IsNullOrEmpty(passphrase))
            {
                if (!CoreSyncConfiguration.FileExists)
                {
                    var directory = Directory.CreateDirectory(GetFullBaseDirectory());

                    directory.Attributes |= FileAttributes.Hidden;

                    var configuration = new CoreSyncConfiguration(passphrase)
                    {
                        EncryptedDirectory = encryptedDirectory
                    };

                    configuration.SerializeToLocalFile(true);

                    Log("Initialization completed successfully.", writeLogEntry: false);
                }
                else
                {
                    Error("Configuration file already exists. Use 'config' command.", writeLogEntry: false);
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
                Error("Configuring requires valid values.", writeLogEntry: false);
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

                Log("Passphrase changed successfully.", writeLogEntry: false);
            }
            else
            {
                Error("Passphrase hasn't been changed.", writeLogEntry: false);
            }
        }

        /// <summary>
        /// Executes functionalities related to command for synchronization of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="subdirectoryPath">
        /// Contains <see cref="string"/> with optional subdirectory path.
        /// </param>
        public static void Synchronize(string subdirectoryPath)
        {
            if (string.IsNullOrEmpty(subdirectoryPath) || SetSubdirectoryPath(subdirectoryPath))
            {
                CoreSyncRepository.SingletonInstance.ProcessSynchronizationOfFileSystemEntries();

                Log("Synchronization completed.", writeLogEntry: false);
            }
        }

        /// <summary>
        /// Executes functionalities related to command for detaching of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="directoryPath">
        /// Contains <see cref="string"/> value with optional directory path.
        /// </param>
        public static void Detach(string directoryPath = null)
        {
            directoryPath = directoryPath ?? GetFullBaseDirectory();

            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);

                Log("Detaching completed successfully.", writeLogEntry: false);
            }
            else
            {
                Error(string.Format("Directory \"{0}\" does not exists.", directoryPath), writeLogEntry: false);
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
            if (!string.IsNullOrEmpty(passphrase))
            {
                var directoryPath = GetFullBaseDirectory();

                if (Directory.Exists(directoryPath))
                {
                    Detach(directoryPath);
                    Initialize(passphrase, encryptedDirectory);
                }
                else
                {
                    Error(string.Format("Directory \"{0}\" does not exists.", directoryPath), writeLogEntry: false);
                }
            }
        }

        /// <summary>
        /// Gets <see cref="string"/> value with current folder path of <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with full directory.
        /// </returns>
        public static string GetApplicationDataFolderPath() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

        /// <summary>
        /// Creates application data <see cref="Directory"/> of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        public static void CreateApplicationDataFolder() => Directory.CreateDirectory(GetApplicationDataFolderPath());

        #endregion

        #region Private Functions

        /// <summary>
        /// Sets <see cref="string"/> with subdirectory path of <see cref="CoreSyncProcessor"/>.
        /// </summary>
        /// <param name="subdirectoryPath">
        /// Contains <see cref="string"/> with subdirectory path.
        /// </param>
        /// <returns>
        /// Returns whether directory exists.
        /// </returns>
        private static bool SetSubdirectoryPath(string subdirectoryPath)
        {
            if (!string.IsNullOrEmpty(subdirectoryPath))
            {
                var separators = new char[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                };

                subdirectoryPath = TrimSeparatorChars(subdirectoryPath, separators);

                var directory = new DirectoryInfo(Path.Combine(WorkingDirectoryPath, subdirectoryPath));

                if (directory.Exists)
                {
                    SubdirectoryPath = directory.FullName;

                    return true;
                }
            }

            Error(string.Format("Subdirectory path \"{0}\" is invalid.", subdirectoryPath), writeLogEntry: false);

            return false;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of separator characters.
        /// </summary>
        /// <param name="path">
        /// Contains <see cref="string"/> value.
        /// </param>
        /// <param name="separatorChar">
        /// Contains <see cref="char[]"/> with directory separators.
        /// </param>
        /// <returns>
        /// Returns the string that remains after all occurrences of the separators.
        /// </returns>
        private static string TrimSeparatorChars(string path, char[] separatorChars)
        {
            foreach (var separator in separatorChars)
            {
                path = path.Trim(separator);
            }

            return path;
        }

        #endregion
    }
}
