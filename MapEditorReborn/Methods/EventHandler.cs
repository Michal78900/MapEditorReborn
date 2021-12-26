namespace MapEditorReborn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
    using API.Enums;
    using API.Extensions;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;
    using MapGeneration;
    using MEC;
    using Mirror.LiteNetLib4Mirror;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    public static partial class Methods
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            SpawnedObjects.Clear();
            ObjectPrefabs.Clear();

            DoorSpawnpoint[] doorList = Object.FindObjectsOfType<DoorSpawnpoint>();
            ObjectPrefabs.Add(ToolGunMode.LczDoor, doorList.First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject);
            ObjectPrefabs.Add(ToolGunMode.HczDoor, doorList.First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject);
            ObjectPrefabs.Add(ToolGunMode.EzDoor, doorList.First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject);

            ObjectPrefabs.Add(ToolGunMode.WorkStation, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "Work Station"));

            ObjectPrefabs.Add(ToolGunMode.ItemSpawnPoint, new GameObject("ItemSpawnPointObject"));
            ObjectPrefabs.Add(ToolGunMode.PlayerSpawnPoint, new GameObject("PlayerSpawnPointObject"));
            ObjectPrefabs.Add(ToolGunMode.RagdollSpawnPoint, new GameObject("RagdollSpawnPointObject"));
            ObjectPrefabs.Add(ToolGunMode.DummySpawnPoint, new GameObject("DummySpawnPointObject"));

            ObjectPrefabs.Add(ToolGunMode.SportShootingTarget, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "sportTargetPrefab"));
            ObjectPrefabs.Add(ToolGunMode.DboyShootingTarget, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "dboyTargetPrefab"));
            ObjectPrefabs.Add(ToolGunMode.BinaryShootingTarget, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "binaryTargetPrefab"));

            ObjectPrefabs.Add(ToolGunMode.Primitive, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "PrimitiveObjectToy"));
            ObjectPrefabs.Add(ToolGunMode.LightSource, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "LightSourceToy"));

            ObjectPrefabs.Add(ToolGunMode.RoomLight, new GameObject("LightControllerObject"));
            ObjectPrefabs.Add(ToolGunMode.Teleporter, new GameObject("TeleportControllerObject"));

            PlayerSpawnPointComponent.RegisterSpawnPoints();

            if (Config.LoadMapOnEvent.OnGenerated.Count != 0)
                Timing.CallDelayed(1f, () => CurrentLoadedMap = GetMapByName(Config.LoadMapOnEvent.OnGenerated[Random.Range(0, Config.LoadMapOnEvent.OnGenerated.Count)]));
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers()
        {
            RoomLightComponent.RegisterFlickerableLights();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted()"/>
        internal static void OnRoundStarted()
        {
            if (Config.LoadMapOnEvent.OnRoundStarted.Count != 0)
                CurrentLoadedMap = GetMapByName(Config.LoadMapOnEvent.OnRoundStarted[Random.Range(0, Config.LoadMapOnEvent.OnRoundStarted.Count)]);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.IsToolGun() && ev.IsThrown)
            {
                ev.IsAllowed = false;

                ToolGuns[ev.Player.CurrentItem.Serial]++;

                if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > 14)
                {
                    ToolGuns[ev.Player.CurrentItem.Serial] = 0;
                }

                ToolGunMode mode = ToolGuns[ev.Player.CurrentItem.Serial];

                ev.Player.ShowHint(!ev.Player.IsAimingDownWeapon && ev.Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>", 1f);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            if (!ev.Shooter.CurrentItem.IsToolGun())
                return;

            ev.IsAllowed = false;

            // Creating an object
            if (ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
            {
                Vector3 forward = ev.Shooter.CameraTransform.forward;
                if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
                {
                    ToolGunMode mode = ToolGuns[ev.Shooter.CurrentItem.Serial];

                    if (mode == ToolGunMode.RoomLight)
                    {
                        Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                        if (SpawnedObjects.FirstOrDefault(x => x is RoomLightComponent light && light.ForcedRoomType == colliderRoom.Type) != null)
                        {
                            ev.Shooter.ShowHint("There can be only one Light Controller per one room type!");
                            return;
                        }
                    }

                    if (ev.Shooter.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                    {
                        SpawnPropertyObject(hit.point, prefab);
                    }
                    else
                    {
                        SpawnObject(hit.point, mode);
                    }
                }

                return;
            }

            if (TryGetMapObject(ev.Shooter, out MapEditorObject mapObject))
            {
                // Deleting the object
                if (!ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                {
                    DeleteObject(ev.Shooter, mapObject);
                    return;
                }
            }

            // Copying to the ToolGun
            if (!ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                CopyObject(ev.Shooter, mapObject);
                return;
            }

            // Selecting the object
            if (ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                SelectObject(ev.Shooter, mapObject);
                return;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.Base.GetComponent<ShootingTargetComponent>() == null)
                return;

            if (ev.TargetButton == ShootingTargetButton.Remove)
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs)"/>
        internal static void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if (ev.Pickup.Base.name.Contains("CustomSchematic"))
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="FileSystemWatcher.OnChanged(FileSystemEventArgs)"/>
        internal static void OnFileChanged(object sender, FileSystemEventArgs ev)
        {
            if (!Config.EnableFileSystemWatcher)
                return;

            string fileName = Path.GetFileNameWithoutExtension(ev.Name);

            if (fileName == CurrentLoadedMap?.Name)
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    try
                    {
                        Log.Debug("Trying to deserialize the file... (called by FileSytemWatcher)", Config.Debug);
                        CurrentLoadedMap = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(ev.FullPath));
                        CurrentLoadedMap.Name = fileName;
                    }
                    catch (Exception e)
                    {
                        Log.Error($"You did something wrong in your MapSchematic file!\n{e.Message}");
                    }
                });
            }
        }

        #region Variables

        /// <summary>
        /// Gets or sets currently loaded <see cref="MapSchematic"/>.
        /// </summary>
        public static MapSchematic CurrentLoadedMap
        {
            get => _mapSchematic;
            set
            {
                LoadMap(value);
            }
        }

        /// <summary>
        /// The list containing objects that are a part of currently loaded <see cref="MapSchematic"/>.
        /// </summary>
        public static List<MapEditorObject> SpawnedObjects = new List<MapEditorObject>();

        /// <summary>
        /// The dictionary that stores currently selected <see cref="ToolGunMode"/> by <see cref="Inventory.SyncItemInfo.Serial"/>.
        /// </summary>
        internal static Dictionary<ushort, ToolGunMode> ToolGuns = new Dictionary<ushort, ToolGunMode>();

        public static Dictionary<ToolGunMode, GameObject> ObjectPrefabs = new Dictionary<ToolGunMode, GameObject>();

        /// <summary>
        /// Gets the name of a variable used for selecting the objects.
        /// </summary>
        public static string SelectedObjectSessionVarName { get; } = "MapEditorReborn_SelectedObject";

        /// <summary>
        /// Gets the name of a variable used for copying the objects.
        /// </summary>
        public static string CopiedObjectSessionVarName { get; } = "MapEditorReborn_CopiedObject";

        private static MapSchematic _mapSchematic;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;

        #endregion
    }
}