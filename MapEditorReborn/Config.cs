namespace MapEditorReborn
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using Exiled.API.Interfaces;

    /// <summary>
    /// The plugin's config.
    /// </summary>
    public class Config : IConfig
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
        /// Gets the possible map's names that may be loaded.
        /// </summary>
        [Description("Should any map be loaded automatically. If there are multiple, the random one will be choosen.")]
        public List<string> LoadMapsOnStart { get; private set; } = new List<string>();
    }
}
