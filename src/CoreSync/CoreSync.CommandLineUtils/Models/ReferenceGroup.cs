// Copyright (c) Thorsten A. Weintz. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace CoreSync.CommandLineUtils.Models
{
    struct ReferenceGroup
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets <see cref="string[]"/> with reference names of <see cref="ReferenceGroup"/>.
        /// </summary>
        public string[] Names { get; set; }

        /// <summary>
        /// Gets or sets <see cref="string"/> with prefix value of <see cref="ReferenceGroup"/>.
        /// </summary>
        public string Prefix { get; set; }

        #endregion
    }
}
