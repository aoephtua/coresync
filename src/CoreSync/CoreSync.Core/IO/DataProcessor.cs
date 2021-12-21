// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CryptLib;
using CoreSync.CryptLib.Core;
using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace CoreSync.Core.IO
{
    public static class DataProcessor
    {
        #region Private Members

        /// <summary>
        /// Contains <see cref="int"/> value with salt length of <see cref="DataProcessor"/>.
        /// </summary>
        private const int saltLength = 32;

        #endregion

        #region Public Functions

        /// <summary>
        /// Gets instance of <see cref="List{Tuple{string, string}}"/> with entries of <see cref="DataProcessor"/>.
        /// </summary>
        /// <param name="directory">
        /// Contains <see cref="string"/> value with source directory path.
        /// </param>
        /// <param name="onlyFiles">
        /// Contains whether result entries are limited to files.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="List{Tuple{string, string}}"/>.
        /// </returns>
        public static List<Tuple<string, string>> GetEntries(string directory, bool onlyFiles = false)
        {
            var entries = new List<Tuple<string, string>>();

            if (Directory.Exists(directory))
            {
                foreach (var fileSystemEntry in Directory.GetFileSystemEntries(directory, "*", SearchOption.AllDirectories))
                {
                    if (new FileInfo(fileSystemEntry) is FileInfo fileInfo && fileInfo.Exists)
                    {
                        entries.Add(new Tuple<string, string>(fileInfo.Directory.Parent.FullName, Path.Combine(fileInfo.Directory.Name, fileInfo.Name)));
                    }
                    else if (!onlyFiles)
                    {
                        entries.Add(new Tuple<string, string>(new DirectoryInfo(fileSystemEntry).FullName, null));
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Encrypts instance of <see cref="Stream"/> to target path of <see cref="DataProcessor"/>.
        /// </summary>
        /// <param name="stream">
        /// Contains instance of <see cref="Stream"/> with source data.
        /// </param>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with target file name.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public static bool Encrypt(Stream stream, string fileName, string passphrase)
        {
            try
            {
                Directory.CreateDirectory(new FileInfo(fileName).DirectoryName);

                using (var encryptedStream = new FileStream(fileName, FileMode.Create))
                {
                    if (stream != null && stream.Length > 0)
                    {
                        var salt = SymmetricCoreCryptor.GetSalt(saltLength);

                        IOProcessor.Write(salt, encryptedStream);

                        new AesCoreCryptor().Encrypt(stream, encryptedStream, passphrase, salt);
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

        /// <summary>
        /// Encrypts data of <see cref="FileStream"/> to target path of <see cref="DataProcessor"/>.
        /// </summary>
        /// <param name="sourceFileName">
        /// Contains <see cref="string"/> value with source file name.
        /// </param>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with target file name.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns whether encryption process succeeded.
        /// </returns>
        public static bool Encrypt(string sourceFileName, string fileName, string passphrase)
        {
            if (File.Exists(sourceFileName))
            {
                try
                {
                    using (var fileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
                    {
                        return Encrypt(fileStream, fileName, passphrase);
                    }
                }
                catch (Exception e)
                {
                    CoreSyncProcessor.Log(e);
                }
            }

            return false;
        }

        /// <summary>
        /// Decrypt data of <see cref="FileStream"/> to instance of <see cref="Stream"/>.
        /// </summary>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with source file name.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        /// <returns>
        /// Returns instance of <see cref="Stream"/>.
        /// </returns>
        public static Stream Decrypt(string fileName, string passphrase)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (var encryptedStream = File.Open(fileName, FileMode.Open))
                    {
                        if (IOProcessor.Read(encryptedStream, saltLength) is byte[] salt && salt.Length > 0)
                        {
                            var encryptedStreamWithoutSalt = new MemoryStream();
                            encryptedStream.CopyTo(encryptedStreamWithoutSalt);
                            IOProcessor.ResetStream(encryptedStreamWithoutSalt);

                            var decryptedStream = new MemoryStream();
                            new AesCoreCryptor().Decrypt(encryptedStreamWithoutSalt, decryptedStream, passphrase, salt);

                            decryptedStream.Seek(0, SeekOrigin.Begin);

                            return decryptedStream;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return null;
        }

        /// <summary>
        /// Decrypt data of <see cref="FileStream"/> to instance of <see cref="FileStream"/>.
        /// </summary>
        /// <param name="targetFileName">
        /// Contains <see cref="string"/> value with target file name.
        /// </param>
        /// <param name="fileName">
        /// Contains <see cref="string"/> value with source file name.
        /// </param>
        /// <param name="passphrase">
        /// Contains <see cref="string"/> value with passphrase.
        /// </param>
        public static bool Decrypt(string targetFileName, string fileName, string passphrase)
        {
            try
            {
                Directory.CreateDirectory(new FileInfo(targetFileName).DirectoryName);

                using (var fileStream = File.Create(targetFileName))
                {
                    var dataStream = Decrypt(fileName, passphrase);

                    if (dataStream != null)
                    {
                        dataStream.CopyTo(fileStream);

                        dataStream.Dispose();

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return false;
        }

        /// <summary>
        /// Deletes <see cref="File"/> or <see cref="Directory"/> of target path.
        /// </summary>
        /// <param name="fullName">
        /// Contains <see cref="string"/> value with target full name.
        /// </param>
        /// <param name="withEmptyParentDirectories">
        /// Contains whether empty parent directories should be deleted.
        /// </param>
        /// <returns>
        /// Returns whether deletion process succeeded.
        /// </returns>
        public static bool Delete(string fullName, bool withEmptyParentDirectories = false)
        {
            string directoryPath = null;

            try
            {
                if (new FileInfo(fullName) is FileInfo fileEntry && fileEntry.Exists)
                {
                    if (withEmptyParentDirectories && Directory.GetFileSystemEntries(fileEntry.Directory.FullName).Length == 1)
                    {
                        directoryPath = fileEntry.Directory.FullName;
                    }
                    else
                    {
                        fileEntry.Delete();
                    }
                }
                else if (Directory.Exists(fullName))
                {
                    directoryPath = fullName;
                }

                if (directoryPath != null)
                {
                    Directory.Delete(withEmptyParentDirectories ? GetNoneEmptyParentDirectory(directoryPath) : directoryPath, true);
                }

                return true;
            }
            catch (Exception e)
            {
                CoreSyncProcessor.Log(e);
            }

            return false;
        }

        /// <summary>
        /// Combines <see cref="string[]"/> with path values.
        /// </summary>
        /// <param name="basePath">
        /// Contains <see cref="string"/> value with base path.
        /// </param>
        /// <param name="paths">
        /// Contains <see cref="string[]"/> with paths of the path.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with target path.
        /// </returns>
        public static string Combine(string basePath, params string[] paths)
        {
            foreach (var path in paths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    basePath = Path.Combine(basePath, path);
                }
            }

            return basePath;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Gets <see cref="string"/> value with none empty parent directory.
        /// </summary>
        /// <param name="path">
        /// Contains <see cref="string"/> value with child path.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> value with valid path.
        /// </returns>
        private static string GetNoneEmptyParentDirectory(string path)
        {
            var parentDirectoryPath = new DirectoryInfo(path).Parent.FullName;

            if (Directory.GetFileSystemEntries(parentDirectoryPath).Length > 1)
            {
                return path;
            }
            else
            {
                return GetNoneEmptyParentDirectory(parentDirectoryPath);
            }
        }

        #endregion
    }
}
