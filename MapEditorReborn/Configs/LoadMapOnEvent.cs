﻿// -----------------------------------------------------------------------
// <copyright file="LoadMapOnEvent.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn
{
    using System.Collections.Generic;

    /// <summary>
    /// The LoadMapOnEvent config.
    /// </summary>
    public sealed class LoadMapOnEvent
    {
        /// <summary>
        /// Gets a list of possible maps.
        /// </summary>
        public List<string> OnGenerated { get; private set; } = new List<string>();

        /// <summary>
        /// Gets a list of possible maps.
        /// </summary>
        public List<string> OnRoundStarted { get; private set; } = new List<string>();

        public List<string> OnWarheadDetonated { get; private set; } = new List<string>();
    }
}
