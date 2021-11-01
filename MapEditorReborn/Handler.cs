namespace MapEditorReborn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
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
    public static partial class Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            SpawnedObjects.Clear();

            DoorSpawnpoint[] doorList = Object.FindObjectsOfType<DoorSpawnpoint>();
            LczDoorObj = doorList.First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject;
            HczDoorObj = doorList.First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject;
            EzDoorObj = doorList.First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject;

            WorkstationObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "Work Station");

            ItemSpawnPointObj = new GameObject("ItemSpawnPointObject");
            PlayerSpawnPointObj = new GameObject("PlayerSpawnPointObject");
            RagdollSpawnPointObj = new GameObject("RagdollSpawnPointObject");

            SportShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "sportTargetPrefab");
            DboyShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "dboyTargetPrefab");
            BinaryShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "binaryTargetPrefab");

            LightControllerObj = new GameObject("LightControllerObject");
            TeleporterObj = new GameObject("TeleportControllerObject");

            if (Config.LoadMapOnEvent.OnGenerated.Count != 0)
                CurrentLoadedMap = GetMapByName(Config.LoadMapOnEvent.OnGenerated[Random.Range(0, Config.LoadMapOnEvent.OnGenerated.Count)]);

            if (CurrentLoadedMap == null || !CurrentLoadedMap.RemoveDefaultSpawnPoints)
                return;

            List<string> spawnPointTags = new List<string>()
            {
                "SP_049",
                "SCP_096",
                "SP_106",
                "SP_173",
                "SCP_939",
                "SP_CDP",
                "SP_RSC",
                "SP_GUARD",
                "SP_MTF",
                "SP_CI",
            };

            foreach (string tag in spawnPointTags)
            {
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(tag))
                {
                    if (gameObject.GetComponent<PlayerSpawnPointComponent>() == null)
                        Object.Destroy(gameObject);
                }
            }
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

                if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > 11)
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

                    if (mode == ToolGunMode.LightController)
                    {
                        Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                        if (SpawnedObjects.FirstOrDefault(x => x is LightControllerComponent light && (Map.FindParentRoom(x.gameObject) == colliderRoom || light.Base.RoomType == colliderRoom.Type)) != null)
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnActivatingWorkstation(ActivatingWorkstationEventArgs)"/>
        internal static void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev)
        {
            if (ev.WorkstationController.TryGetComponent(out WorkStationObjectComponent workStation) && !workStation.Base.IsInteractable)
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.Base.gameObject.GetComponent<ShootingTargetComponent>() == null)
                return;

            if (ev.TargetButton == ShootingTargetButton.Remove)
                ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs)"/>
        internal static void OnChangingIntoGrenade(ChangingIntoGrenadeEventArgs ev)
        {
            if ((bool)ev.Pickup.Base.transform?.parent.name.Contains("CustomSchematic"))
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
                    }
                    catch (Exception e)
                    {
                        Log.Error($"You did something wrong in your MapSchematic file.{e.Message}");
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
                _mapSchematic = value;
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
        public static Dictionary<ushort, ToolGunMode> ToolGuns = new Dictionary<ushort, ToolGunMode>();

        /// <summary>
        /// The Light Contaiment Zone door prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject LczDoorObj;

        /// <summary>
        /// The Heavy Contaiment Zone door prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject HczDoorObj;

        /// <summary>
        /// The Entrance Zone door prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject EzDoorObj;

        /// <summary>
        /// The Workstation prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject WorkstationObj;

        /// <summary>
        /// The ItemSpawnPoint prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject ItemSpawnPointObj;

        /// <summary>
        /// The PlayerSpawnPoint prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject PlayerSpawnPointObj;

        /// <summary>
        /// The RagdollSpawnPoint prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject RagdollSpawnPointObj;

        /// <summary>
        /// The SportShootingTarget prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject SportShootingTargetObj;

        /// <summary>
        /// The DboyShootingTarget prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject DboyShootingTargetObj;

        /// <summary>
        /// The BinaryShootingTarget prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject BinaryShootingTargetObj;

        /// <summary>
        /// The LightController prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject LightControllerObj;

        /// <summary>
        /// The Teleported prefab <see cref="GameObject"/>.
        /// </summary>
        public static GameObject TeleporterObj;

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