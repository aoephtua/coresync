// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils.Attributes;
using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [HelpOption]
    abstract class CommandBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string"/> value with directory option of <see cref="CommandBase"/>.
        /// </summary>
        [Directory]
        [Option(Template = "-dir|--directory", Description = "Contains the base directory path. Default value is current directory path.")]
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets <see cref="bool"/> with flag to display current application and framework versions of <see cref="CommandBase"/>.
        /// </summary>
        [Option(Template = "-v|--version", Description = "Displays the current application and framework versions.")]
        public bool Version { get; set; } 

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets or sets instance of <see cref="CommandLineApplication"/>.
        /// </summary>
        protected CommandLineApplication App { get; private set; }

        /// <summary>
        /// Gets or sets whether execution of <see cref="CommandBase"/> is valid.
        /// </summary>
        protected virtual bool Valid => true;

        #endregion

        #region Protected Functions

        /// <summary>
        /// Raises on execution of <see cref="CommandLineApplication"/> instance.
        /// </summary>
        /// <param name="app">
        /// Contains instance of <see cref="CommandLineApplication"/>.
        /// </param>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        protected virtual int OnExecute(CommandLineApplication app)
        {
            App = app;

            if (Version)
            {
                GetVersions();
            }
            else
            {
                if (!string.IsNullOrEmpty(Directory))
                {
                    CoreSyncProcessor.WorkingDirectoryPath = CoreSyncProcessor.GetDataDirectory(Directory);
                }

                if (CoreSyncProcessor.LogNotification == null)
                {
                    CoreSyncProcessor.LogNotification += (logEntry) =>
                    {
                        Console.WriteLine(logEntry.ToString());
                    };
                }

                try
                {
                    if (Valid)
                    {
                        Execute();
                    }
                }
                catch (Exception e)
                {
                    CoreSyncProcessor.Log(e);
                }
            }

            return 0;
        }

        /// <summary>
        /// Executes command functionalities of <see cref="CommandBase"/>.
        /// </summary>
        protected abstract void Execute();

        #endregion

        #region Private Functions

        /// <summary>
        /// Gets current application and framework versions of <see cref="CommandBase"/>.
        /// </summary>
        private void GetVersions()
        {
            WriteAssemblyVersions();

            foreach (var reference in new string[] { "CommandLineUtils", "Core", "CryptLib" })
            {
                var name = string.Format("CoreSync.{0}.dll", reference);
                var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);

                if (Assembly.LoadFile(fullName) is Assembly assembly)
                {
                    WriteAssemblyVersions(assembly);
                }
            }
        }

        /// <summary>
        /// Gets custom <see cref="Attribute"/> of <see cref="Assembly"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>.
        /// </param>
        /// <returns></returns>
        private T GetCustomAttribute<T>(Assembly assembly) where T : Attribute =>
            assembly.GetCustomAttribute<T>();

        /// <summary>
        /// Gets <see cref="string"/> with informational version of <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> with current application version.
        /// </returns>
        private string GetVersion(Assembly assembly) =>
            GetCustomAttribute<AssemblyInformationalVersionAttribute>(assembly).InformationalVersion;

        /// <summary>
        /// Gets <see cref="string"/> with framework name of <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> with framework name.
        /// </returns>
        private string GetFrameworkName(Assembly assembly) =>
            GetCustomAttribute<TargetFrameworkAttribute>(assembly).FrameworkName;


        /// <summary>
        /// Writes <see cref="Console"/> line of <see cref="Assembly"/> versions.
        /// </summary>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>. Default value is the process executable in the default application domain.
        /// </param>
        private void WriteAssemblyVersions(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }

            var name = assembly.ManifestModule.Name;
            var version = GetVersion(assembly);
            var frameworkDisplayName = GetFrameworkName(assembly);

            Console.WriteLine("{0}: v{1} ({2})", name, version, frameworkDisplayName);
        }

        #endregion
    }
}
