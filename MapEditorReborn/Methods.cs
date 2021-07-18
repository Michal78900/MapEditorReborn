namespace MapEditorReborn
{
    using System.IO;
    using System.Linq;
    using API;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Loader;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// Contains mostly methods for spawning objects, saving/loading maps.
    /// </summary>
    public partial class Handler
    {
        /// <summary>
        /// Loads the <see cref="MapSchematic"/> map.
        /// It also may be used for reloading the map.
        /// </summary>
        /// <param name="map"><see cref="MapSchematic"/> to load.</param>
        public static void LoadMap(MapSchematic map)
        {
            Log.Debug("Trying to load the map...", Config.Debug);

            foreach (GameObject spawnedObj in SpawnedObjects)
            {
                // NetworkServer.Destroy(spawnedObj) doesn't call OnDestroy methods in components for some reason.
                Object.Destroy(spawnedObj);
            }

            SpawnedObjects.Clear();

            foreach (GameObject indicator in Indicators)
            {
                NetworkServer.Destroy(indicator);
            }

            Indicators.Clear();

            Log.Debug("Destroyed all map's GameObjects and indicators.", Config.Debug);

            if (map == null)
            {
                Log.Debug("Map is null. Returning...", Config.Debug);
                return;
            }

            foreach (DoorObject door in map.Doors)
            {
                Log.Debug($"Trying to spawn door at {door.Position}...", Config.Debug);
                SpawnDoor(door);
            }

            if (map.Doors.Count > 0)
                Log.Debug("All doors have been successfully spawned!", Config.Debug);

            foreach (WorkStationObject workstation in map.WorkStations)
            {
                Log.Debug($"Spawning workstation at {workstation.Position}...", Config.Debug);
                SpawnWorkStation(workstation);
            }

            if (map.WorkStations.Count > 0)
                Log.Debug("All workstations have been successfully spawned!", Config.Debug);

            foreach (ItemSpawnPointObject itemSpawnPoint in map.ItemSpawnPoints)
            {
                SpawnItemSpawnPoint(itemSpawnPoint);
            }

            foreach (PlayerSpawnPointObject playerSpawnPoint in map.PlayerSpawnPoints)
            {
                SpawnPlayerSpawnPoint(playerSpawnPoint);
            }

            Log.Debug("All GameObject have been spawned and the MapSchematic has been fully loaded!", Config.Debug);
        }

        /// <summary>
        /// Saves the map to a file.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public static void SaveMap(string name)
        {
            Log.Debug("Trying to save the map...", Config.Debug);

            MapSchematic map = new MapSchematic
            {
                Name = name,
            };

            Log.Debug($"Map name set to \"{map.Name}\"", Config.Debug);

            foreach (GameObject gameObject in SpawnedObjects)
            {
                Log.Debug($"Trying to save GameObject at {gameObject.transform.position}...", Config.Debug);

                Room room = Map.FindParentRoom(gameObject);

                switch (gameObject.name)
                {
                    case "LCZ BreakableDoor(Clone)":
                    case "HCZ BreakableDoor(Clone)":
                    case "EZ BreakableDoor(Clone)":
                        {
                            map.Doors.Add(gameObject.GetComponent<DoorVariant>().ConvertToDoorObject());
                            break;
                        }

                    case "Work Station(Clone)":
                        {
                            map.WorkStations.Add(new WorkStationObject(
                                gameObject.transform.position,
                                gameObject.transform.eulerAngles,
                                gameObject.transform.localScale,
                                room.Type));

                            break;
                        }

                    case "PlayerSpawnPointObject(Clone)":
                        {
                            map.PlayerSpawnPoints.Add(new PlayerSpawnPointObject(
                                gameObject.tag.ConvertToRoleType(),
                                room.Type == RoomType.Surface ? gameObject.transform.position : room.transform.InverseTransformPoint(gameObject.transform.position),
                                room.Type));

                            break;
                        }

                    case "ItemSpawnPointObject(Clone)":
                        {
                            ItemSpawnPointComponent itemSpawnPointComponent = gameObject.GetComponent<ItemSpawnPointComponent>();

                            map.ItemSpawnPoints.Add(new ItemSpawnPointObject(
                                itemSpawnPointComponent.ItemName,
                                room.Type == RoomType.Surface ? gameObject.transform.position : room.transform.InverseTransformPoint(gameObject.transform.position),
                                gameObject.transform.eulerAngles,
                                room.Type));

                            break;
                        }
                }
            }

            string path = Path.Combine(MapEditorReborn.PluginDir, $"{map.Name}.yml");

            Log.Debug($"Path to file set to: {path}", Config.Debug);

            bool prevValue = Config.EnableFileSystemWatcher;
            if (prevValue)
                Config.EnableFileSystemWatcher = false;

            Log.Debug("Trying to serialize the MapSchematic...", Config.Debug);

            File.WriteAllText(path, Loader.Serializer.Serialize(map));

            Log.Debug("MapSchematic has been successfully saved to a file!", Config.Debug);

            Timing.CallDelayed(1f, () => Config.EnableFileSystemWatcher = prevValue);
        }

        /// <summary>
        /// Gets the <see cref="MapSchematic"/> by it's name.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <returns><see cref="MapSchematic"/> if the file with the map was found, otherwise <see langword="null"/>.</returns>
        public static MapSchematic GetMapByName(string mapName)
        {
            string path = Path.Combine(MapEditorReborn.PluginDir, $"{mapName}.yml");

            if (!File.Exists(path))
                return null;

            return Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));
        }

        /// <summary>
        /// Spawns a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorObject"/> which is used to spawn a door.</param>
        /// <returns><see cref="GameObject"/> of the spawned door.</returns>
        public static GameObject SpawnDoor(DoorObject door)
        {
            GameObject gameObject = Object.Instantiate(door.DoorType.GetDoorObjectByType(), door.Position, Quaternion.Euler(door.Rotation));
            gameObject.transform.localScale = door.Scale;

            DoorVariant doorVaraintComponent = gameObject.GetComponent<DoorVariant>();
            doorVaraintComponent.NetworkTargetState = door.IsOpen;
            doorVaraintComponent.ServerChangeLock(DoorLockReason.SpecialDoorFeature, door.IsLocked);
            doorVaraintComponent.RequiredPermissions.RequiredPermissions = door.KeycardPermissions;

            BreakableDoor breakableDoor = doorVaraintComponent as BreakableDoor;
            breakableDoor._ignoredDamageSources = door.IgnoredDamageSources;
            breakableDoor._maxHealth = door.DoorHealth;

            DoorObjectComponent doorObjectComponent = gameObject.AddComponent<DoorObjectComponent>();
            doorObjectComponent.OpenOnWarheadActivation = door.OpenOnWarheadActivation;

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Spawns a workstation.
        /// </summary>
        /// <param name="workStation">The <see cref="WorkStationObject"/> to spawn.</param>
        /// <returns><see cref="GameObject"/> of the spawned workstation.</returns>
        public static GameObject SpawnWorkStation(WorkStationObject workStation)
        {
            GameObject gameObject = Object.Instantiate(WorkstationObj, workStation.Position, Quaternion.Euler(workStation.Rotation));
            gameObject.transform.localScale = workStation.Scale;

            gameObject.AddComponent<WorkStation>();

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Spawns a ItemSpawnPoint.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointObject"/> to spawn.</param>
        /// <returns><see cref="GameObject"/> of the spawned ItemSpawnPoint.</returns>
        public static GameObject SpawnItemSpawnPoint(ItemSpawnPointObject itemSpawnPoint)
        {
            GameObject gameObject = Object.Instantiate(ItemSpawnPointObj, GetRelativePosition(itemSpawnPoint.Position, itemSpawnPoint.RoomType), GetRelativeRotation(itemSpawnPoint.Rotation, itemSpawnPoint.RoomType));

            ItemSpawnPointComponent itemSpawnPointComponent = gameObject.AddComponent<ItemSpawnPointComponent>();
            itemSpawnPointComponent.ItemName = itemSpawnPoint.Item;

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Spawns a PlayerSpawnPoint.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointObject"/> to spawn.</param>
        /// <returns><see cref="GameObject"/> of the spawned PlayerSpawnPoint.</returns>
        public static GameObject SpawnPlayerSpawnPoint(PlayerSpawnPointObject playerSpawnPoint)
        {
            GameObject gameObject = Object.Instantiate(PlayerSpawnPointObj, GetRelativePosition(playerSpawnPoint.Position, playerSpawnPoint.RoomType), Quaternion.Euler(Vector3.zero));
            gameObject.tag = playerSpawnPoint.RoleType.ConvertToSpawnPointTag();

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Spawns a general <see cref="GameObject"/>.
        /// Used by the ToolGun.
        /// </summary>
        /// <param name="position">The postition of the spawned object.</param>
        /// <param name="toolGunMode">The current <see cref="ToolGunMode"/>.</param>
        /// <returns>The spawned <see cref="GameObject"/>.</returns>
        public static GameObject SpawnObject(Vector3 position, ToolGunMode toolGunMode)
        {
            GameObject gameObject = Object.Instantiate(toolGunMode.GetObjectByMode(), position, Quaternion.Euler(0f, 0f, 0f));

            switch (toolGunMode)
            {
                case ToolGunMode.LczDoor:
                case ToolGunMode.HczDoor:
                case ToolGunMode.EzDoor:
                    {
                        gameObject.AddComponent<DoorObjectComponent>();
                        break;
                    }

                case ToolGunMode.WorkStation:
                    {
                        gameObject.AddComponent<WorkStation>();
                        break;
                    }

                case ToolGunMode.ItemSpawnPoint:
                    {
                        gameObject.AddComponent<ItemSpawnPointComponent>();
                        break;
                    }

                case ToolGunMode.PlayerSpawnPoint:
                    {
                        gameObject.tag = "SP_173";
                        gameObject.transform.position += Vector3.up * 0.1f;
                        break;
                    }
            }

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Spawns a copy of selected object by a ToolGun.
        /// </summary>
        /// <param name="position">Postion of spawned property object.</param>
        /// <param name="prefab">The <see cref="GameObject"/> from which the copy will be spawned.</param>
        public static void SpawnPropertyObject(Vector3 position, GameObject prefab)
        {
            GameObject gameObject = Object.Instantiate(prefab, position, prefab.transform.rotation);

            SpawnedObjects.Add(gameObject);

            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Gets a position relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="position">The object position.</param>
        /// <param name="roomType">The <see cref="RoomType"/> from which the <see cref="Room"/> object will be choosed.</param>
        /// <returns>Global position relative to the <see cref="Room"/>. If the <paramref name="roomType"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="position"/> will be retured with no changes.</returns>
        public static Vector3 GetRelativePosition(Vector3 position, RoomType roomType)
        {
            if (roomType == RoomType.Surface)
            {
                return position;
            }
            else
            {
                return Map.Rooms.First(x => x.Type == roomType).transform.TransformPoint(position);
            }
        }

        /// <summary>
        /// Gets a rotation relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="rotation">The object rotation.</param>
        /// <param name="roomType">The <see cref="RoomType"/> from which the <see cref="Room"/> object will be choosed.</param>
        /// <returns>Global rotation relative to the <see cref="Room"/>. If the <paramref name="roomType"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="rotation"/> will be retured with no changes.</returns>
        public static Quaternion GetRelativeRotation(Vector3 rotation, RoomType roomType)
        {
            if (roomType == RoomType.Surface)
            {
                return Quaternion.Euler(rotation);
            }
            else
            {
                return Map.Rooms.First(x => x.Type == roomType).transform.rotation * Quaternion.Euler(rotation);
            }
        }
    }
}
