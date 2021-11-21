namespace MapEditorReborn
{
#pragma warning disable SA1649
    using System;
    using System.IO;
    using Exiled.API.Features;
    using HarmonyLib;

    using MapEvent = Exiled.Events.Handlers.Map;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main <see cref="MapEditorReborn"/> plugin class.
    /// </summary>
    public class MapEditorReborn : Plugin<Config, Translation>
    {
        /// <summary>
        /// The <see langword="static"/> instance of the <see cref="MapEditorReborn"/>.
        /// </summary>
        public static MapEditorReborn Singleton;

        /// <summary>
        /// Gets the MapEditorReborn parent folder path.
        /// </summary>
        public static string PluginDir { get; } = Path.Combine(Paths.Configs, "MapEditorReborn");

        /// <summary>
        /// Gets the folder path in which the maps are stored.
        /// </summary>
        public static string MapsDir { get; } = Path.Combine(PluginDir, "Maps");

        /// <summary>
        /// Gets the folder path in which the schematics are stored.
        /// </summary>
        public static string SchematicsDir { get; } = Path.Combine(PluginDir, "Schematics");

        private Harmony harmony;

        private FileSystemWatcher fileSystemWatcher;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Singleton = this;

            if (!Directory.Exists(PluginDir))
            {
                Log.Warn("MapEditorReborn parent directory does not exist. Creating...");
                Directory.CreateDirectory(PluginDir);
            }

            if (!Directory.Exists(MapsDir))
            {
                Log.Warn("Maps directory does not exist. Creating...");
                Directory.CreateDirectory(MapsDir);
            }

            if (!Directory.Exists(SchematicsDir))
            {
                Log.Warn("Schematics directory does not exist. Creating...");
                Directory.CreateDirectory(SchematicsDir);
            }

            // TO BE REMOVED IN NEXT RELEASE
            foreach (string path in Directory.GetFiles(PluginDir, "*.yml"))
            {
                string newPath = Path.Combine(MapsDir, Path.GetFileName(path));

                if (File.Exists(Path.Combine(MapsDir, Path.GetFileName(path))))
                    continue;

                Log.Warn($"\"{Path.GetFileName(path)}\" is in a wrong location. Moving it to the correct one...");
                File.Move(path, newPath);
            }
            // TO BE REMOVED IN NEXT RELEASE

            harmony = new Harmony($"michal78900.mapEditorReborn-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            MapEvent.Generated += Handler.OnGenerated;
            ServerEvent.WaitingForPlayers += Handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted += Handler.OnRoundStarted;

            PlayerEvent.DroppingItem += Handler.OnDroppingItem;
            PlayerEvent.Shooting += Handler.OnShooting;

            PlayerEvent.InteractingShootingTarget += Handler.OnInteractingShootingTarget;
            MapEvent.ChangingIntoGrenade += Handler.OnChangingIntoGrenade;

            if (Config.EnableFileSystemWatcher)
            {
                fileSystemWatcher = new FileSystemWatcher(MapsDir)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = "*.yml",
                    EnableRaisingEvents = true,
                };

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
            ServerEvent.WaitingForPlayers -= Handler.OnWaitingForPlayers;
            ServerEvent.RoundStarted -= Handler.OnRoundStarted;

            PlayerEvent.DroppingItem -= Handler.OnDroppingItem;
            PlayerEvent.Shooting -= Handler.OnShooting;

            PlayerEvent.InteractingShootingTarget -= Handler.OnInteractingShootingTarget;
            MapEvent.ChangingIntoGrenade -= Handler.OnChangingIntoGrenade;

            if (fileSystemWatcher != null)
                fileSystemWatcher.Changed -= Handler.OnFileChanged;

            base.OnDisabled();
        }

        /// <inheritdoc/>
        public override string Name => "MapEditorReborn";

        /// <inheritdoc/>
        public override string Author => "Michal78900 (original idea by Killers0992)";

        /// <inheritdoc/>
        public override Version Version => new Version(1, 2, 3);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(3, 7, 2);
    }
}
