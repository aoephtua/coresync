﻿// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.CommandLineUtils.Commands;
using CoreSync.CommandLineUtils.Models;
using CoreSync.Core;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;

#endregion

namespace CoreSync.CommandLineUtils
{
    [HelpOption]
    [Command(Name = CoreSyncProcessor.ApplicationName, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
    [Subcommand(typeof(InitCommand))]
    [Subcommand(typeof(ConfigCommand))]
    [Subcommand(typeof(FilterCommand))]
    [Subcommand(typeof(SyncCommand))]
    [Subcommand(typeof(DetachCommand))]
    [Subcommand(typeof(ResetCommand))]
    public class Bootstraper
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="bool"/> with flag to display current application and framework versions of <see cref="Bootstraper"/>.
        /// </summary>
        [Option(Template = "-v|--version", Description = "Displays the current application and framework versions.")]
        public bool Version { get; set; }

        /// <summary>
        /// Gets or sets <see cref="List{string}"/> with remaining arguments of <see cref="CommandLineApplication"/>.
        /// </summary>
        public List<string> RemainingArguments { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Main function that is bootstrapping the core processor when the CLI application is started.
        /// </summary>
        /// <param name="args">
        /// Contains instance of <see cref="string[]"/> with startup arguments.
        /// </param>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        public static int ExecuteApplication(string[] args) => CommandLineApplication.Execute<Bootstraper>(args);

        #endregion

        #region Private Functions

        /// <summary>
        /// Raises on execution of <see cref="CommandLineApplication"/> instance.
        /// </summary>
        /// <returns>
        /// Returns exit code for instance of <see cref="CommandLineApplication"/>.
        /// </returns>
        private int OnExecute() 
        {
            if (Version)
            {
                GetVersions();
            }
            else
            {
                return CommandLineApplication.Execute<SyncCommand>(RemainingArguments?.ToArray());
            }

            return 0;
        }

        /// <summary>
        /// Gets current application and framework versions of <see cref="CommandBase"/>.
        /// </summary>
        private void GetVersions()
        {
            WriteAssemblyVersions();

            foreach (var group in GetReferenceGroups())
            {
                foreach (var reference in group.Names)
                {
                    var name = PrepareReferenceName(reference, group.Prefix);
                    var fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);

                    if (Assembly.LoadFile(fullName) is Assembly assembly)
                    {
                        WriteAssemblyVersions(assembly);
                    }
                }
            }
        }

        /// <summary>
        /// Creates instance of <see cref="List{ReferenceGroup}"/> with reference entries.
        /// </summary>
        /// <returns>
        /// Returns instance of <see cref="List{ReferenceGroup}"/>.
        /// </returns>
        private List<ReferenceGroup> GetReferenceGroups()
        {
            return new List<ReferenceGroup>()
            {
                new ReferenceGroup()
                {
                    Names = new string[] 
                    { 
                        "CommandLineUtils",
                        "Core",
                        "CryptLib" 
                    },
                    Prefix = "CoreSync."
                },
                new ReferenceGroup()
                {
                    Names = new string[]
                    { 
                        "McMaster.Extensions.CommandLineUtils",
                        "System.Security.Cryptography.ProtectedData"
                    }
                }
            };
        }

        /// <summary>
        /// Prepares <see cref="string"/> values with reference name.
        /// </summary>
        /// <param name="names">
        /// Contains <see cref="string"/> with reference name.
        /// </param>
        /// <param name="prefix">
        /// Contains optional <see cref="string"/> value with prefix.
        /// </param>
        /// <returns>
        /// Returns prepared <see cref="string"/>.
        /// </returns>
        private static string PrepareReferenceName(string name, string prefix = "") =>
            string.Format("{0}{1}.dll", prefix, name);

        /// <summary>
        /// Gets custom <see cref="Attribute"/> of <see cref="Assembly"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>.
        /// </param>
        /// <returns></returns>
        private static T GetCustomAttribute<T>(Assembly assembly) where T : Attribute
        {
            try
            {
                return assembly.GetCustomAttribute<T>();
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Gets <see cref="string"/> with informational version of <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>.
        /// </param>
        /// <returns>
        /// Returns <see cref="string"/> with current application version.
        /// </returns>
        private static string GetVersion(Assembly assembly) =>
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
        private static string GetFrameworkName(Assembly assembly)
        {
            var attribute = GetCustomAttribute<TargetFrameworkAttribute>(assembly);

            if (attribute != null)
            {
                return attribute.FrameworkName;
            }

            return null;
        }

        /// <summary>
        /// Writes <see cref="Console"/> line of <see cref="Assembly"/> versions.
        /// </summary>
        /// <param name="assembly">
        /// Contains instance of <see cref="Assembly"/>. Default value is the process executable in the default application domain.
        /// </param>
        private static void WriteAssemblyVersions(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }

            var name = assembly.ManifestModule.Name;
            var version = GetVersion(assembly).Split(new char[] { ' ', '+' })[0];
            var frameworkDisplayName = GetFrameworkName(assembly);

            if (frameworkDisplayName != null)
            {
                frameworkDisplayName = string.Format(" ({0})", frameworkDisplayName);
            }

            Console.WriteLine("{0}: v{1}{2}", name, version, frameworkDisplayName);
        }

        #endregion
    }
}
