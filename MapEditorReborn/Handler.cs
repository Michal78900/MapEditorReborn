namespace MapEditorReborn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;
    using MapGeneration;
    using MEC;
    using Mirror;
    using Mirror.LiteNetLib4Mirror;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    public partial class Handler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            SpawnedObjects.Clear();
            Indicators.Clear();

            LczDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject;
            HczDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject;
            EzDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject;

            WorkstationObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "Work Station");

            ItemSpawnPointObj = new GameObject("ItemSpawnPointObject");
            PlayerSpawnPointObj = new GameObject("PlayerSpawnPointObject");
            RagdollSpawnPointObj = new GameObject("RagdollSpawnPointObject");

            SportShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "sportTargetPrefab");
            DboyShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "dboyTargetPrefab");
            BinaryShootingTargetObj = LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "binaryTargetPrefab");

            LightControllerObj = new GameObject("LightControllerObject");

            if (Config.LoadMapsOnStart.Count != 0)
            {
                CurrentLoadedMap = GetMapByName(Config.LoadMapsOnStart[Random.Range(0, Config.LoadMapsOnStart.Count)]);
            }

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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.IsToolGun())
            {
                ev.IsAllowed = false;

                ToolGuns[ev.Player.CurrentItem.Serial]++;

                if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > 10)
                {
                    ToolGuns[ev.Player.CurrentItem.Serial] = 0;
                }

                ev.Player.ShowHint($"<b>{ToolGuns[ev.Player.CurrentItem.Serial]}</b>", 1f);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShooting(ShootingEventArgs ev)
        {
            if (!ev.Shooter.CurrentItem.IsToolGun())
                return;

            ev.IsAllowed = false;

            const string copyObject = "MapEditorReborn_CopyObject";

            Vector3 forward = ev.Shooter.CameraTransform.forward;
            if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                // Creating an object
                if (ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                {
                    ToolGunMode mode = ToolGuns[ev.Shooter.CurrentItem.Serial];

                    if (mode == ToolGunMode.LightController && SpawnedObjects.FirstOrDefault(x => x is LightControllerComponent && Map.FindParentRoom(x.gameObject) == ev.Shooter.CurrentRoom) != null)
                    {
                        ev.Shooter.ShowHint("There can be only one Light Controller per one room type!");
                        return;
                    }

                    if (ev.Shooter.TryGetSessionVariable(copyObject, out GameObject copyGameObject))
                    {
                        Log.Debug(ev.Shooter.SessionVariables[copyObject] != null);
                        Log.Info(copyGameObject != null);

                        SpawnPropertyObject(hit.point, copyGameObject);
                    }
                    else
                    {
                        SpawnObject(hit.point, mode);
                    }

                    return;
                }

                MapEditorObject mapObject = hit.collider.GetComponentInParent<MapEditorObject>();

                /*
                if (mapObject != null)
                {
                    // Deleting the object by it's indicator
                    if (Indicators.TryGetValue(parent, out SpawnedObjectComponent primitive) && !ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                    {
                        SpawnedObjects.Remove(primitive);
                        NetworkServer.Destroy(primitive);

                        Indicators.Remove(parent);
                        NetworkServer.Destroy(parent);

                        return;
                    }

                    if (primitive != null)
                    {
                        Log.Debug(primitive.name, Config.Debug);

                        parent = primitive;
                    }
                }
                */

                // Copying to the ToolGun
                if (!ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
                {
                    if (mapObject != null && SpawnedObjects.Contains(mapObject))
                    {
                        if (!ev.Shooter.SessionVariables.ContainsKey(copyObject))
                        {
                            ev.Shooter.SessionVariables.Add(copyObject, Object.Instantiate(mapObject));
                        }
                        else
                        {
                            ev.Shooter.SessionVariables[copyObject] = Object.Instantiate(mapObject);
                        }

                        ev.Shooter.ShowHint("Object properties have been copied to the ToolGun.");
                    }
                    else
                    {
                        ev.Shooter.SessionVariables.Remove(copyObject);
                        ev.Shooter.ShowHint("ToolGun has been reseted to default settings.");
                    }

                    return;
                }

                // Selecting the object
                if (ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
                {
                    if (mapObject != null && SpawnedObjects.Contains(mapObject))
                    {
                        ev.Shooter.ShowGamgeObjectHint(mapObject.gameObject);

                        if (!ev.Shooter.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
                        {
                            ev.Shooter.SessionVariables.Add(SelectedObjectSessionVarName, mapObject);
                        }
                        else
                        {
                            ev.Shooter.SessionVariables[SelectedObjectSessionVarName] = mapObject;
                        }
                    }
                    else
                    {
                        ev.Shooter.SessionVariables.Remove(SelectedObjectSessionVarName);
                        ev.Shooter.ShowHint("Object have been unselected");
                    }

                    return;
                }

                if (mapObject == null || !SpawnedObjects.Contains(mapObject))
                    return;

                // Deleting the object
                if (!ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                {
                    if (ev.Shooter.TryGetSessionVariable(SelectedObjectSessionVarName, out GameObject selectedGameObject) && selectedGameObject == mapObject)
                    {
                        ev.Shooter.SessionVariables.Remove(SelectedObjectSessionVarName);
                        ev.Shooter.ShowHint(string.Empty, 0.1f);
                    }

                    SpawnedObjects.Remove(mapObject);
                    NetworkServer.Destroy(mapObject.gameObject);

                    return;
                }
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.GetComponent<ShootingTargetComponent>() != null && ev.TargetButton == InventorySystem.Items.Firearms.Utilities.ShootingTarget.TargetButton.Remove)
            {
                ev.IsAllowed = false;
            }
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
        /// The list containing objects, which indicate where <see cref="ItemSpawnPointObject"/> and <see cref="PlayerSpawnPointObject"/> are located.
        /// </summary>
        public static Dictionary<GameObject, GameObject> Indicators = new Dictionary<GameObject, GameObject>();

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
        /// Gets the name of a variable used for selecting the objects.
        /// </summary>
        public static string SelectedObjectSessionVarName { get; } = "MapEditorReborn_SelectedObject";

        private static MapSchematic _mapSchematic;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;

        #endregion
    }
}