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
    using Interactables.Interobjects.DoorUtils;
    using MapGeneration;
    using MEC;
    using Mirror;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    public partial class Handler
    {
        /// <summary>
        /// The list containing objects that are a part of currently loaded <see cref="MapSchematic"/>.
        /// </summary>
        public static List<GameObject> SpawnedObjects = new List<GameObject>();

        /// <summary>
        /// The list containing objects, which indicate where <see cref="ItemSpawnPointObject"/> and <see cref="PlayerSpawnPointObject"/> are located.
        /// </summary>
        public static Dictionary<GameObject, GameObject> Indicators = new Dictionary<GameObject, GameObject>();

        /// <summary>
        /// The dictionary that stores currently selected <see cref="ToolGunMode"/> by <see cref="Inventory.SyncItemInfo.uniq"/>.
        /// </summary>
        public static Dictionary<int, ToolGunMode> ToolGuns = new Dictionary<int, ToolGunMode>();

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

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            SpawnedObjects.Clear();
            Indicators.Clear();

            LczDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.ToUpper().Contains("LCZ")).TargetPrefab.gameObject;
            HczDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.ToUpper().Contains("HCZ")).TargetPrefab.gameObject;
            EzDoorObj = Object.FindObjectsOfType<DoorSpawnpoint>().First(x => x.TargetPrefab.name.ToUpper().Contains("EZ")).TargetPrefab.gameObject;
            WorkstationObj = NetworkManager.singleton.spawnPrefabs.Find(p => p.gameObject.name.ToUpper().Contains("WORK"));

            ItemSpawnPointObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ItemSpawnPointObj.name = "ItemSpawnPointObject";
            ItemSpawnPointObj.transform.localScale = Vector3.zero;

            PlayerSpawnPointObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            PlayerSpawnPointObj.name = "PlayerSpawnPointObject";
            PlayerSpawnPointObj.transform.localScale = Vector3.zero;

            RagdollSpawnPointObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            RagdollSpawnPointObj.name = "RagdollSpawnPointObject";
            RagdollSpawnPointObj.transform.localScale = Vector3.zero;

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
                    if (!SpawnedObjects.Contains(gameObject))
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
                if (ev.Shooter.HasFlashlightEnabled() && !ev.Shooter.ReferenceHub.weaponManager.NetworksyncZoomed)
                {
                    ToolGunMode mode = ToolGuns[ev.Shooter.CurrentItem.uniq];

                    if ((mode == ToolGunMode.LczDoor || mode == ToolGunMode.HczDoor || mode == ToolGunMode.EzDoor) && Map.FindParentRoom(hit.collider.gameObject).Type != RoomType.Surface)
                    {
                        ev.Shooter.ShowHint("<size=25><color=#B80000><b>You can't spawn doors inside the Facility, because it will crash your game!</b></color></size>");
                        return;
                    }

                    if (ev.Shooter.TryGetSessionVariable(copyObject, out GameObject copyGameObject))
                    {
                        SpawnPropertyObject(hit.point, copyGameObject);
                    }
                    else
                    {
                        SpawnObject(hit.point, mode);
                    }

                    return;
                }

                GameObject parent = hit.collider.GetComponentInParent<DoorVariant>()?.gameObject ?? // Door                 (DoorObject)
                                    hit.collider.GetComponentInParent<WorkStation>()?.gameObject ?? // Workstation          (WorkstationObject)
                                    hit.collider.GetComponentInParent<NetworkIdentity>()?.gameObject ?? // Dummy Indicator  (PlayerSpawnPointObject and RagdollSpawnPointObject)
                                    hit.collider.GetComponentInParent<Pickup>()?.gameObject; // Pickup indicator            (ItemSpawnPointObject)

                if (parent != null)
                {
                    // Deleting the object by it's indicator
                    if (Indicators.TryGetValue(parent, out GameObject primitive) && !ev.Shooter.HasFlashlightEnabled() && !ev.Shooter.ReferenceHub.weaponManager.NetworksyncZoomed)
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

                // Copying to the ToolGun
                if (!ev.Shooter.HasFlashlightEnabled() && ev.Shooter.ReferenceHub.weaponManager.NetworksyncZoomed)
                {
                    if (parent != null && SpawnedObjects.Contains(parent))
                    {
                        if (!ev.Shooter.SessionVariables.ContainsKey(copyObject))
                        {
                            ev.Shooter.SessionVariables.Add(copyObject, Object.Instantiate(parent, Vector3.zero, Quaternion.identity));
                        }
                        else
                        {
                            ev.Shooter.SessionVariables[copyObject] = Object.Instantiate(parent, Vector3.zero, Quaternion.identity);
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
                if (ev.Shooter.HasFlashlightEnabled() && ev.Shooter.ReferenceHub.weaponManager.NetworksyncZoomed)
                {
                    if (parent != null && SpawnedObjects.Contains(parent))
                    {
                        ev.Shooter.ShowGamgeObjectHint(parent);

                        if (!ev.Shooter.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
                        {
                            ev.Shooter.SessionVariables.Add(SelectedObjectSessionVarName, parent);
                        }
                        else
                        {
                            ev.Shooter.SessionVariables[SelectedObjectSessionVarName] = parent;
                        }
                    }
                    else
                    {
                        ev.Shooter.SessionVariables.Remove(SelectedObjectSessionVarName);
                        ev.Shooter.ShowHint("Object have been unselected");
                    }

                    return;
                }

                if (parent == null || !SpawnedObjects.Contains(parent))
                    return;

                // Deleting the object
                if (!ev.Shooter.HasFlashlightEnabled() && !ev.Shooter.ReferenceHub.weaponManager.NetworksyncZoomed)
                {
                    if (ev.Shooter.TryGetSessionVariable(SelectedObjectSessionVarName, out GameObject selectedGameObject) && selectedGameObject == parent)
                    {
                        ev.Shooter.SessionVariables.Remove(SelectedObjectSessionVarName);
                        ev.Shooter.ShowHint(string.Empty, 0.1f);
                    }

                    SpawnedObjects.Remove(parent);
                    NetworkServer.Destroy(parent);

                    return;
                }
            }
        }

        private static List<Player> tempList = new List<Player>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnReloadingWeapon(ReloadingWeaponEventArgs)"/>
        internal static void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (ev.Player.CurrentItem.IsToolGun())
            {
                ev.IsAllowed = false;

                if (tempList.Contains(ev.Player))
                    return;

                tempList.Add(ev.Player);

                ToolGuns[ev.Player.CurrentItem.uniq]++;

                if ((int)ToolGuns[ev.Player.CurrentItem.uniq] > 6)
                {
                    ToolGuns[ev.Player.CurrentItem.uniq] = 0;
                }

                ev.Player.ShowHint($"<b>{ToolGuns[ev.Player.CurrentItem.uniq]}</b>", 1f);

                Timing.CallDelayed(1.5f, () => tempList.Remove(ev.Player));
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnPickingUpItem(PickingUpItemEventArgs)"/>
        internal static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (Indicators.ContainsKey(ev.Pickup.gameObject))
            {
                ev.IsAllowed = false;
            }
        }

        /// <summary>
        /// Gets the name of a variable used for selecting the objects.
        /// </summary>
        public static string SelectedObjectSessionVarName { get; } = "MapEditorReborn_SelectedObject";

        private static MapSchematic _mapSchematic;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}
