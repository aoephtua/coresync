// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using CoreSync.Core.Model;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#endregion

namespace CoreSync.CommandLineUtils.Commands
{
    [Command(Name = "filter",  Description = "Add, remove or list filters. See 'filter -?' for more details.", ThrowOnUnexpectedArgument = false)]
    [Subcommand(typeof(ListFiltersCommand))]
    [Subcommand(typeof(AddFiltersCommand))]
    [Subcommand(typeof(RemoveFiltersCommand))]
    class FilterCommand : SecuredCommandBase
    {
        #region Protected Functions

        /// <summary>
        /// Executes command functionalities of <see cref="FilterCommand"/>.
        /// </summary>
        protected override void Execute()
        {
            Console.WriteLine("Error: You must specify a subcommand.");

            App.ShowHelp();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Outputs processed filter message of <see cref="FilterCommand"/>.
        /// </summary>
        /// <param name="prefix">
        /// Contains <see cref="string"/> value with prefix.
        /// </param>
        /// <param name="cnt">
        /// Contains <see cref="int"/> value with processed count.
        /// </param>
        private static void OutputProcessedFilterMessage(string prefix, int cnt)
        {
            Console.WriteLine("{0} {1} filter {2}.", prefix, cnt == 0 ? "no" : cnt.ToString(), cnt == 1 ? "entry" : "entries");
        }

        #endregion

        #region Subcommands

        [Command(Name = "ls", Description = "List filters of configuration file.")]
        class ListFiltersCommand : SecuredCommandBase
        {
            #region Protected Functions

            /// <summary>
            /// Executes command functionalities of <see cref="ListFiltersCommand"/>.
            /// </summary>
            protected override void Execute()
            {
                var filters = CoreSyncConfiguration.SingletonInstance.Filters;

                for (var i = 0; i < filters.Count; i++)
                {
                    Console.WriteLine("{0}: {1}", i + 1, filters[i]);
                }
            }

            #endregion
        }

        [Command(Name = "add", Description = "Add filters to configuration file. Regular expressions are supported.")]
        class AddFiltersCommand : SecuredCommandBase
        {
            #region Private Properties

            /// <summary>
            /// Gets <see cref="string[]"/> with filter values of <see cref="AddFiltersCommand"/>.
            /// </summary>
            [Argument(0, Description = "Contains the filter values.")]
            private string[] FilterValues { get; }

            #endregion

            #region Protected Functions

            /// <summary>
            /// Executes command functionalities of <see cref="AddFiltersCommand"/>.
            /// </summary>
            protected override void Execute()
            {
                var cnt = 0;

                foreach (var filterValue in FilterValues)
                {
                    if (IsValidRegex(filterValue) && !CoreSyncConfiguration.SingletonInstance.Filters.Any(x => x == filterValue))
                    {
                        CoreSyncConfiguration.SingletonInstance.Filters.Add(filterValue);

                        cnt++;
                    }
                }

                if (cnt > 0)
                {
                    CoreSyncConfiguration.SingletonInstance.SerializeToLocalFile(true);
                }

                OutputProcessedFilterMessage("Added", cnt);
            }

            #endregion

            #region Private Functions

            /// <summary>
            /// Checks whether <see cref="Regex"/> pattern is valid.
            /// </summary>
            /// <param name="pattern">
            /// Contains <see cref="string"/> value with <see cref="Regex"/> pattern.
            /// </param>
            /// <returns>
            /// Returns whether pattern is valid.
            /// </returns>
            private bool IsValidRegex(string pattern)
            {
                if (string.IsNullOrEmpty(pattern)) return false;

                try
                {
                    Regex.Match(String.Empty, pattern);
                }
                catch (ArgumentException)
                {
                    return false;
                }

                return true;
            }

            #endregion
        }

        [Command(Name = "rm", Description = "Remove filters of configuration file.")]
        class RemoveFiltersCommand : SecuredCommandBase
        {
            #region Private Properties

            /// <summary>
            /// Gets <see cref="string[]"/> with filter numbers of <see cref="RemoveFiltersCommand"/>.
            /// </summary>
            [Argument(0, Description = "Contains the filter numbers.")]
            private string[] FilterNumbers { get; }

            #endregion

            #region Protected Functions

            /// <summary>
            /// Executes command functionalities of <see cref="RemoveFiltersCommand"/>.
            /// </summary>
            protected override void Execute()
            {
                if (CoreSyncConfiguration.SingletonInstance.Filters.Any())
                {
                    var cnt = 0;

                    if (FilterNumbers.Length > 0)
                    {
                        cnt = DeleteFilterValues();
                    }
                    else
                    {
                        if (Prompt.GetYesNo("Do you want to delete all filters?", true))
                        {
                            cnt = CoreSyncConfiguration.SingletonInstance.Filters.Count;

                            CoreSyncConfiguration.SingletonInstance.Filters.Clear();
                        }
                    }

                    if (cnt > 0)
                    {
                        CoreSyncConfiguration.SingletonInstance.SerializeToLocalFile(true);
                    }

                    OutputProcessedFilterMessage("Removed", cnt);
                }
            }

            #endregion

            #region Private Functions

            /// <summary>
            /// Deletes <see cref="string"/> filter values of <see cref="FilterCommand"/>.
            /// </summary>
            /// <returns>
            /// Returns <see cref="int"/> value with count of deleted values.
            /// </returns>
            private int DeleteFilterValues()
            {
                var filterValues = new List<string>();

                foreach (var filterNumber in FilterNumbers)
                {
                    if (int.TryParse(filterNumber, out int number) && --number < CoreSyncConfiguration.SingletonInstance.Filters.Count)
                    {
                        filterValues.Add(CoreSyncConfiguration.SingletonInstance.Filters[number]);
                    }
                }

                foreach (var filterValue in filterValues)
                {
                    CoreSyncConfiguration.SingletonInstance.Filters.Remove(filterValue);
                }

                return filterValues.Count;
            }

            #endregion
        }

        #endregion
    }
}
