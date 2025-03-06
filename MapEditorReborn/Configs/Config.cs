// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Configs
{
    using System.ComponentModel;
    using API.Enums;
    using Exiled.API.Interfaces;

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
        public bool Debug { get; set; } = false;

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
        /// Gets a value indicating whether the schematic .zip files should be automatically extracted when the server starts.
        /// </summary>
        [Description("Whether the schematic .zip files should be automatically extracted when the server starts.")]
        public bool AutoExtractSchematics { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether gets or sets a value whether the plugin tracking is enabled. This is used to count how many servers are using the plugin.
        /// </summary>
        [Description("Whether the plugin tracking is enabled. This is used to count how many servers are using the plugin.")]
        public bool PluginTracking { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether the object will be auto selected when spawning it.
        /// </summary>
        [Description("Whether the object will be auto selected when spawning it.")]
        public bool AutoSelect { get; private set; } = true;

        /// <summary>
        /// Gets a LoadMapOnEvent class.
        /// </summary>
        [Description("Option to load maps, when the specific event is called. If there are multiple maps, the random one will be choosen. Use UNLOAD to unload the map.")]
        public LoadMapOnEvent LoadMapOnEvent { get; private set; } = new ();

        /// <summary>
        /// Gets the mode used for selecting maps when using LoadMapOnEvent.
        /// </summary>
        [Description("The mode used for selecting maps when using the LoadMapOnEvent. Setting this to Random will pick a random map from the available maps. Setting this to Merge will automatically merge multiple maps into one and load all of them.")]
        public LoadMapOnEventMode LoadMapOnEventMode { get; private set; } = LoadMapOnEventMode.Random;
    }
}
