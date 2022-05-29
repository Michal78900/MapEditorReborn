﻿// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn
{
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using UnityEngine;

    /// <summary>
    /// The plugin's config.
    /// </summary>
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled or not.
        /// </summary>
        [Description("Is the plugin enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the plugin's debug mode is enabled or not.
        /// </summary>
        [Description("Is the debug mode enabled.")]
        public bool Debug { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the showing indicators on object spawn is enabled.
        /// </summary>
        [Description("Should object indicator be shown when you spawn an object.")]
        public bool ShowIndicatorOnSpawn { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether the plugin's <see cref="System.IO.FileSystemWatcher"/> is enabled or not.
        /// </summary>
        [Description("Enables FileSystemWatcher in this plugin. What it does is when you manually change values in a currently loaded map file, after saving the file the plugin will automatically reload the map in-game with the new changes so you won't need to do it yourself.")]
        public bool EnableFileSystemWatcher { get; internal set; } = false;

        /// <summary>
        /// Gets a delay between spawning each block of a custom schematic.
        /// </summary>
        [Description("The delay (in seconds) between spawning each block of a custom schematic. Setting this to -1 will disable it.")]
        public float SchematicBlockSpawnDelay { get; private set; } = 0f;

        /// <summary>
        /// Gets a LoadMapOnEvent class.
        /// </summary>
        [Description("Option to load maps, when the specific event is called. If there are multiple maps, the random one will be choosen.")]
        public LoadMapOnEvent LoadMapOnEvent { get; private set; } = new LoadMapOnEvent();
    }
}
