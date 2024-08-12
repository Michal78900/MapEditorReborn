// -----------------------------------------------------------------------
// <copyright file="MapEditorReborn.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Configs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using HarmonyLib;
    using EventHandler = global::MapEditorReborn.Events.Handlers.Internal.EventHandler;
    using MapEvent = Exiled.Events.Handlers.Map;
    using PlayerEvent = Exiled.Events.Handlers.Player;
    using ServerEvent = Exiled.Events.Handlers.Server;
    using WarheadEvent = Exiled.Events.Handlers.Warhead;

    /// <summary>
    /// The main <see cref="MapEditorReborn"/> plugin class.
    /// </summary>
    public class MapEditorReborn : Plugin<Config, Translation>
    {
        private Harmony _harmony;
        private FileSystemWatcher _fileSystemWatcher;
        private Thread _merClock;

        /// <summary>
        /// Gets the <see langword="static"/> instance of the <see cref="MapEditorReborn"/>.
        /// </summary>
        public static MapEditorReborn Singleton { get; private set; }

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
            else
            {
                foreach (string path in Directory.GetFiles(SchematicsDir))
                {
                    if (path.EndsWith(".json"))
                    {
                        string schematicName = Path.GetFileNameWithoutExtension(path);
                        string directoryPath = Path.Combine(SchematicsDir, schematicName);
                        if (!Directory.Exists(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        File.Move(path, Path.Combine(directoryPath, schematicName) + ".json");

                        Log.Warn($"{schematicName}.json has been moved to its own folder. Please put an entire schematic directory, not a single file!");
                        continue;
                    }

                    if (Config.AutoExtractSchematics && path.EndsWith(".zip"))
                    {
                        Task.Run(() =>
                        {
                            string schematicName = Path.GetFileNameWithoutExtension(path);
                            string directoryPath = Path.Combine(SchematicsDir, schematicName);
                            if (Directory.Exists(directoryPath))
                                Directory.Delete(directoryPath, true);

                            Log.Warn($"Extracting {schematicName}.zip...");
                            ZipFile.ExtractToDirectory(path, SchematicsDir);

                            Log.Warn($"{schematicName}.zip has been successfully extracted!");
                            File.Delete(path);
                        });
                    }
                }
            }

            MapEvent.Generated += EventHandler.OnGenerated;
            ServerEvent.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            ServerEvent.RoundStarted += EventHandler.OnRoundStarted;
            MapEvent.Decontaminating += EventHandler.OnDecontaminating;
            WarheadEvent.Detonated += EventHandler.OnWarheadDetonated;

            PlayerEvent.Shooting += EventHandler.OnShootingDoor;
            PlayerEvent.InteractingShootingTarget += EventHandler.OnInteractingShootingTarget;
            PlayerEvent.DamagingShootingTarget += EventHandler.OnDamagingShootingTarget;
            PlayerEvent.SearchingPickup += EventHandler.OnSearchingPickup;
            PlayerEvent.PickingUpItem += EventHandler.OnPickingUpItem;
            PlayerEvent.InteractingLocker += EventHandler.OnInteractingLocker;

            PlayerEvent.AimingDownSight += ToolGunHandler.OnAimingDownSight;
            PlayerEvent.TogglingWeaponFlashlight += ToolGunHandler.OnTogglingWeaponFlashlight;
            PlayerEvent.UnloadingWeapon += ToolGunHandler.OnUnloadingWeapon;
            PlayerEvent.DroppingItem += ToolGunHandler.OnDroppingItem;
            PlayerEvent.Shooting += ToolGunHandler.OnShooting;

            PlayerEvent.ChangingItem += GravityGunHandler.OnChangingItem;
            PlayerEvent.DroppingItem += GravityGunHandler.OnDroppingItem;
            PlayerEvent.DryfiringWeapon += GravityGunHandler.OnShootingGun;
            PlayerEvent.ReloadingWeapon += GravityGunHandler.OnReloading;

            PlayerEvent.Spawning += API.Features.Objects.PlayerSpawnPointObject.OnPlayerSpawning;

            _harmony = new Harmony($"michal78900.mapEditorReborn-{DateTime.Now.Ticks}");
            _harmony.PatchAll();

            if (Config.EnableFileSystemWatcher)
            {
                _fileSystemWatcher = new FileSystemWatcher(MapsDir)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Filter = "*.yml",
                    EnableRaisingEvents = true,
                };

                _fileSystemWatcher.Changed += EventHandler.OnFileChanged;

                Log.Debug("FileSystemWatcher enabled!");
            }

            if (Config.PluginTracking)
            {
                _merClock = new Thread(ServerCountHandler.Loop)
                {
                    Name = "MER Clock",
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true,
                };

                _merClock.Start();
            }

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Singleton = null;

            MapEvent.Generated -= EventHandler.OnGenerated;
            ServerEvent.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
            ServerEvent.RoundStarted -= EventHandler.OnRoundStarted;
            MapEvent.Decontaminating -= EventHandler.OnDecontaminating;
            WarheadEvent.Detonated -= EventHandler.OnWarheadDetonated;

            PlayerEvent.Shooting -= EventHandler.OnShootingDoor;
            PlayerEvent.InteractingShootingTarget -= EventHandler.OnInteractingShootingTarget;
            PlayerEvent.DamagingShootingTarget -= EventHandler.OnDamagingShootingTarget;
            PlayerEvent.SearchingPickup -= EventHandler.OnSearchingPickup;
            PlayerEvent.PickingUpItem -= EventHandler.OnPickingUpItem;
            PlayerEvent.InteractingLocker -= EventHandler.OnInteractingLocker;

            PlayerEvent.AimingDownSight -= ToolGunHandler.OnAimingDownSight;
            PlayerEvent.TogglingWeaponFlashlight -= ToolGunHandler.OnTogglingWeaponFlashlight;
            PlayerEvent.UnloadingWeapon -= ToolGunHandler.OnUnloadingWeapon;
            PlayerEvent.DroppingItem -= ToolGunHandler.OnDroppingItem;
            PlayerEvent.Shooting -= ToolGunHandler.OnShooting;

            PlayerEvent.ChangingItem -= GravityGunHandler.OnChangingItem;
            PlayerEvent.DroppingItem -= GravityGunHandler.OnDroppingItem;
            PlayerEvent.DryfiringWeapon -= GravityGunHandler.OnShootingGun;
            PlayerEvent.ReloadingWeapon -= GravityGunHandler.OnReloading;

            PlayerEvent.Spawning -= API.Features.Objects.PlayerSpawnPointObject.OnPlayerSpawning;

            _harmony.UnpatchAll();

            if (_fileSystemWatcher != null)
                _fileSystemWatcher.Changed -= EventHandler.OnFileChanged;

            _merClock?.Abort();

            base.OnDisabled();
        }

        /// <inheritdoc/>
        public override string Name => "MapEditorReborn";

        /// <inheritdoc/>
        public override string Author => "Michal78900, NekoDev Team";

        /// <inheritdoc/>
        public override Version Version { get; } = new (3, 1, 9);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new (8, 9, 5);
    }
}