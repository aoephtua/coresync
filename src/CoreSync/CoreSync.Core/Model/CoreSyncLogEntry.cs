// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#region Using Directives

using System;
using System.Globalization;

#endregion

namespace CoreSync.Core.Model
{
    public class CoreSyncLogEntry
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="DateTime"/> value of <see cref="CoreSyncLogEntry"/>.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets <see cref="DateTime"/> of <see cref="CoreSyncLogEntry"/> as formatted value.
        /// </summary>
        public string OutputDate => Date.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets or sets <see cref="CoreSyncLogLevel"/> value of <see cref="CoreSyncLogEntry"/>.
        /// </summary>
        public CoreSyncLogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets <see cref="string"/> value with data of <see cref="CoreSyncLogEntry"/>.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets whether only data of <see cref="CoreSyncLogEntry"/> should be outputted.
        /// </summary>
        public bool DataOnly { get; set; }

        #endregion

        #region Public Functions

        /// <summary>
        /// Returns a string that represents the current instance of <see cref="CoreSyncLogEntry"/>.
        /// </summary>
        /// <returns>
        /// Returns <see cref="string"/> value with formatted object data.
        /// </returns>
        public override string ToString()
        {
            var logLevelName = Enum.GetName(typeof(CoreSyncLogLevel), LogLevel);

            return DataOnly ? string.Format("{0}: {1}", logLevelName, Data) : string.Format("{0}  {1}  {2}", OutputDate, logLevelName, Data);
        }

        #endregion
    }
}
