namespace MapEditorReborn
{
#pragma warning disable SA1649
    using System;
    using System.IO;
    using Exiled.API.Features;
    using HarmonyLib;

    using MapEvent = Exiled.Events.Handlers.Map;
    using PlayerEvent = Exiled.Events.Handlers.Player;

    /// <summary>
    /// The main <see cref="MapEditorReborn"/> plugin class.
    /// </summary>
    public class MapEditorReborn : Plugin<Config>
    {
        /// <summary>
        /// The <see langword="static"/> instance of the <see cref="MapEditorReborn"/>.
        /// </summary>
        public static MapEditorReborn Singleton;

        /// <summary>
        /// Gets the folder path in which the schematics are stored.
        /// </summary>
        public static string PluginDir { get; } = Path.Combine(Paths.Configs, "MapEditorReborn");

        private Harmony harmony;

        private FileSystemWatcher fileSystemWatcher;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton = this;

            if (!Directory.Exists(PluginDir))
            {
                Log.Warn("MapEditorReborn directory does not exist. Creating...");
                Directory.CreateDirectory(PluginDir);
            }

            harmony = new Harmony($"michal78900.mapEditorReborn-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            MapEvent.Generated += Handler.OnGenerated;

            PlayerEvent.DroppingItem += Handler.OnDroppingItem;
            PlayerEvent.Shooting += Handler.OnShooting;
            PlayerEvent.ReloadingWeapon += Handler.OnReloadingWeapon;
            PlayerEvent.PickingUpItem += Handler.OnPickingUpItem;

            if (Config.EnableFileSystemWatcher)
            {
                fileSystemWatcher = new FileSystemWatcher(PluginDir);
                fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileSystemWatcher.Filter = "*.yml";
                fileSystemWatcher.EnableRaisingEvents = true;

                fileSystemWatcher.Changed += Handler.OnFileChanged;

                Log.Debug("FileSystemWatcher enabled!", Config.Debug);
            }

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Singleton = null;
            harmony.UnpatchAll();

            MapEvent.Generated -= Handler.OnGenerated;

            PlayerEvent.DroppingItem -= Handler.OnDroppingItem;
            PlayerEvent.Shooting -= Handler.OnShooting;
            PlayerEvent.ReloadingWeapon -= Handler.OnReloadingWeapon;
            PlayerEvent.PickingUpItem -= Handler.OnPickingUpItem;

            fileSystemWatcher.Changed -= Handler.OnFileChanged;

            base.OnDisabled();
        }

        /// <inheritdoc/>
        public override string Author => "Michal78900 (original idea by Killers0992)";

        /// <inheritdoc/>
        public override Version Version => new Version(0, 4, 1);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(2, 11, 1);
    }
}
