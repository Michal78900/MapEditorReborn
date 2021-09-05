namespace MapEditorReborn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Loader;
    using Interactables.Interobjects.DoorUtils;
    using MEC;
    using Mirror;
    using Mirror.LiteNetLib4Mirror;
    using RemoteAdmin;
    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Contains mostly methods for spawning objects, saving/loading maps.
    /// </summary>
    public partial class Handler
    {
        #region Map Schematic Methods

        /// <summary>
        /// Loads the <see cref="MapSchematic"/> map.
        /// It also may be used for reloading the map.
        /// </summary>
        /// <param name="map"><see cref="MapSchematic"/> to load.</param>
        public static void LoadMap(MapSchematic map)
        {
            Log.Debug("Trying to load the map...", Config.Debug);

            foreach (MapEditorObject mapEditorObject in SpawnedObjects)
            {
                mapEditorObject.Destroy();
            }

            SpawnedObjects.Clear();

            foreach (GameObject indicator in Indicators.Keys)
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

            // Map.Rooms is null at this time, so this delay is required.
            Timing.CallDelayed(0.01f, () =>
            {
                // This MUST be executed first. If the default spawnpoins were destroyed I only have a brief period of time to replace them with a new ones.
                foreach (PlayerSpawnPointObject playerSpawnPoint in map.PlayerSpawnPoints)
                {
                    Log.Debug($"Trying to spawn a player spawn point at {playerSpawnPoint.Position}...", Config.Debug);
                    SpawnPlayerSpawnPoint(playerSpawnPoint);
                }

                if (map.PlayerSpawnPoints.Count > 0)
                    Log.Debug("All player spawn points have been spawned!", Config.Debug);

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
                    Log.Debug($"Trying to spawn a item spawn point at {itemSpawnPoint.Position}...", Config.Debug);
                    SpawnItemSpawnPoint(itemSpawnPoint);
                }

                if (map.ItemSpawnPoints.Count > 0)
                    Log.Debug("All item spawn points have been spawned!", Config.Debug);

                foreach (RagdollSpawnPointObject ragdollSpawnPoint in map.RagdollSpawnPoints)
                {
                    Log.Debug($"Trying to spawn a ragdoll spawn point at {ragdollSpawnPoint.Position}...", Config.Debug);
                    SpawnRagdollSpawnPoint(ragdollSpawnPoint);
                }

                if (map.RagdollSpawnPoints.Count > 0)
                    Log.Debug("All ragdoll spawn points have been spawned!", Config.Debug);

                foreach (ShootingTargetObject shootingTargetObject in map.ShootingTargetObjects)
                {
                    Log.Debug($"Trying to spawn a shooting target at {shootingTargetObject.Position}...", Config.Debug);
                    SpawnShootingTarget(shootingTargetObject);
                }

                if (map.ShootingTargetObjects.Count > 0)
                    Log.Debug("All shooting targets have been spawned!", Config.Debug);

                foreach (LightControllerObject lightControllerObject in map.LightControllerObjects)
                {
                    SpawnLightController(lightControllerObject);
                    Log.Debug($"Trying to spawn a light controller at {lightControllerObject.RoomType}...", Config.Debug);
                }

                if (map.LightControllerObjects.Count > 0)
                    Log.Debug("All shooting targets have been spawned!", Config.Debug);

                Log.Debug("All GameObject have been spawned and the MapSchematic has been fully loaded!", Config.Debug);
            });
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
                RemoveDefaultSpawnPoints = CurrentLoadedMap?.RemoveDefaultSpawnPoints ?? default,
                RagdollRoleNames = CurrentLoadedMap?.RagdollRoleNames ?? default,
            };

            Log.Debug($"Map name set to \"{map.Name}\"", Config.Debug);

            foreach (MapEditorObject spawnedObject in SpawnedObjects)
            {
                Log.Debug($"Trying to save GameObject at {spawnedObject.transform.position}...", Config.Debug);

                if (!(spawnedObject is LightControllerComponent))
                    spawnedObject.CurrentRoom = Map.FindParentRoom(spawnedObject.gameObject);

                switch (spawnedObject)
                {
                    case DoorObjectComponent door:
                        {
                            map.Doors.Add(new DoorObject(
                                door.DoorType,
                                door.RelativePosition,
                                door.RelativeRotation,
                                door.Scale,
                                door.CurrentRoom.Type,
                                door.IsOpen,
                                door.IsLocked,
                                door.DoorPermissions,
                                door.IgnoredDamageTypes,
                                door.MaxHealth,
                                door.OpenOnWarheadActivation));

                            break;
                        }

                    case WorkstationObjectComponent workStation:
                        {
                            map.WorkStations.Add(new WorkStationObject(
                                workStation.RelativePosition,
                                workStation.RelativeRotation,
                                workStation.Scale,
                                workStation.CurrentRoom.Type));

                            break;
                        }

                    case PlayerSpawnPointComponent playerspawnPoint:
                        {
                            map.PlayerSpawnPoints.Add(new PlayerSpawnPointObject(
                                playerspawnPoint.tag.ConvertToRoleType(),
                                playerspawnPoint.RelativePosition,
                                playerspawnPoint.CurrentRoom.Type));

                            break;
                        }

                    case ItemSpawnPointComponent itemSpawnPoint:
                        {
                            map.ItemSpawnPoints.Add(new ItemSpawnPointObject(
                                itemSpawnPoint.ItemName,
                                itemSpawnPoint.RelativePosition,
                                itemSpawnPoint.RelativeRotation,
                                itemSpawnPoint.CurrentRoom.Type,
                                itemSpawnPoint.SpawnChance,
                                itemSpawnPoint.NumberOfItems));

                            break;
                        }

                    case RagdollSpawnPointComponent ragdollSpawnPoint:
                        {
                            string ragdollName = string.Empty;

                            if (CurrentLoadedMap != null && CurrentLoadedMap.RagdollRoleNames.TryGetValue(ragdollSpawnPoint.RagdollRoleType, out List<string> list) && !list.Contains(ragdollSpawnPoint.RagdollName))
                            {
                                ragdollName = ragdollSpawnPoint.RagdollName;
                            }

                            map.RagdollSpawnPoints.Add(new RagdollSpawnPointObject(
                                ragdollName,
                                ragdollSpawnPoint.RagdollRoleType,
                                ragdollSpawnPoint.RagdollDamageType,
                                ragdollSpawnPoint.RelativePosition,
                                ragdollSpawnPoint.RelativeRotation,
                                ragdollSpawnPoint.CurrentRoom.Type));

                            break;
                        }

                    case ShootingTargetComponent shootingTarget:
                        {
                            map.ShootingTargetObjects.Add(new ShootingTargetObject(
                                shootingTarget.TargetType,
                                shootingTarget.RelativePosition,
                                shootingTarget.RelativeRotation,
                                shootingTarget.Scale,
                                shootingTarget.CurrentRoom.Type));

                            break;
                        }

                    case LightControllerComponent lightController:
                        {
                            map.LightControllerObjects.Add(new LightControllerObject(
                                lightController.RoomColor.r,
                                lightController.RoomColor.g,
                                lightController.RoomColor.b,
                                lightController.RoomColor.a,
                                lightController.OnlyWarheadLight,
                                lightController.RoomType));

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

        #endregion

        #region Spawning Objects Methods

        /// <summary>
        /// Spawns a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorObject"/> which is used to spawn a door.</param>
        public static void SpawnDoor(DoorObject door)
        {
            Room room = GetRandomRoom(door.RoomType);
            GameObject gameObject = Object.Instantiate(door.DoorType.GetDoorObjectByType(), GetRelativePosition(door.Position, room), GetRelativeRotation(door.Rotation, room));
            gameObject.transform.localScale = door.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(door.Rotation);

            Door exiledDoor = Door.Get(gameObject.GetComponent<DoorVariant>());
            exiledDoor.IsOpen = door.IsOpen;
            if (door.IsLocked)
                exiledDoor.ChangeLock(DoorLockType.SpecialDoorFeature);
            exiledDoor.RequiredPermissions.RequiredPermissions = door.KeycardPermissions;

            exiledDoor.IgnoredDamageTypes = door.IgnoredDamageSources;
            exiledDoor.MaxHealth = door.DoorHealth;

            var comp = gameObject.AddComponent<DoorObjectComponent>();
            SpawnedObjects.Add(comp);
            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Spawns a workstation.
        /// </summary>
        /// <param name="workStation">The <see cref="WorkStationObject"/> to spawn.</param>
        public static void SpawnWorkStation(WorkStationObject workStation)
        {
            Room room = GetRandomRoom(workStation.RoomType);
            GameObject gameObject = Object.Instantiate(WorkstationObj, GetRelativePosition(workStation.Position, room), GetRelativeRotation(workStation.Rotation, room));
            gameObject.transform.localScale = workStation.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(workStation.Rotation);

            var comp = gameObject.AddComponent<WorkstationObjectComponent>();
            SpawnedObjects.Add(comp);
            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Spawns a ItemSpawnPoint.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointObject"/> to spawn.</param>
        public static void SpawnItemSpawnPoint(ItemSpawnPointObject itemSpawnPoint)
        {
            Room room = GetRandomRoom(itemSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ItemSpawnPointObj, GetRelativePosition(itemSpawnPoint.Position, room), GetRelativeRotation(itemSpawnPoint.Rotation, room));

            gameObject.AddComponent<ObjectRotationComponent>().Init(itemSpawnPoint.Rotation);

            var comp = gameObject.AddComponent<ItemSpawnPointComponent>();
            comp.Init(itemSpawnPoint);
            SpawnedObjects.Add(comp);
        }

        /// <summary>
        /// Spawns a PlayerSpawnPoint.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointObject"/> to spawn.</param>
        public static void SpawnPlayerSpawnPoint(PlayerSpawnPointObject playerSpawnPoint)
        {
            Room room = GetRandomRoom(playerSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(PlayerSpawnPointObj, GetRelativePosition(playerSpawnPoint.Position, room), Quaternion.identity);
            gameObject.tag = playerSpawnPoint.RoleType.ConvertToSpawnPointTag();

            var comp = gameObject.AddComponent<PlayerSpawnPointComponent>();
            SpawnedObjects.Add(comp);
        }

        /// <summary>
        /// Spawns a RagdollSpawnPoint.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointObject"/> to spawn.</param>
        public static void SpawnRagdollSpawnPoint(RagdollSpawnPointObject ragdollSpawnPoint)
        {
            Room room = GetRandomRoom(ragdollSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(RagdollSpawnPointObj, GetRelativePosition(ragdollSpawnPoint.Position, room), GetRelativeRotation(ragdollSpawnPoint.Rotation, room));

            gameObject.AddComponent<ObjectRotationComponent>().Init(ragdollSpawnPoint.Rotation);

            var comp = gameObject.AddComponent<RagdollSpawnPointComponent>();
            comp.Init(ragdollSpawnPoint);
            SpawnedObjects.Add(comp);
        }

        /// <summary>
        /// Spawns a ShootingTarget.
        /// </summary>
        /// <param name="shootingTarget">The <see cref="ShootingTargetObject"/> to spawn.</param>
        public static void SpawnShootingTarget(ShootingTargetObject shootingTarget)
        {
            Room room = GetRandomRoom(shootingTarget.RoomType);
            GameObject gameObject = Object.Instantiate(shootingTarget.GetShootingTargetObjectByType(), GetRelativePosition(shootingTarget.Position, room), GetRelativeRotation(shootingTarget.Rotation, room));
            gameObject.transform.localScale = shootingTarget.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(shootingTarget.Rotation);

            var comp = gameObject.AddComponent<ShootingTargetComponent>();
            comp.Init(shootingTarget);
            SpawnedObjects.Add(comp);
            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Spawns a LightController.
        /// </summary>
        /// <param name="lightController">The <see cref="LightControllerObject"/> to spawn.</param>
        public static void SpawnLightController(LightControllerObject lightController)
        {
            GameObject gameObject = Object.Instantiate(LightControllerObj);

            var comp = gameObject.AddComponent<LightControllerComponent>();
            comp.Init(lightController);
            SpawnedObjects.Add(comp);
        }

        /// <summary>
        /// Spawns a general <see cref="GameObject"/>.
        /// Used by the ToolGun.
        /// </summary>
        /// <param name="position">The postition of the spawned object.</param>
        /// <param name="mode">The current <see cref="ToolGunMode"/>.</param>
        public static void SpawnObject(Vector3 position, ToolGunMode mode)
        {
            GameObject gameObject = Object.Instantiate(mode.GetObjectByMode(), position, Quaternion.identity);
            gameObject.transform.rotation = GetRelativeRotation(Vector3.zero, Map.FindParentRoom(gameObject));

            switch (mode)
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
                        gameObject.AddComponent<WorkstationObjectComponent>();
                        break;
                    }

                case ToolGunMode.ItemSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.1f;
                        gameObject.AddComponent<ItemSpawnPointComponent>().Init();
                        break;
                    }

                case ToolGunMode.PlayerSpawnPoint:
                    {
                        gameObject.tag = "SP_173";
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<PlayerSpawnPointComponent>();
                        break;
                    }

                case ToolGunMode.RagdollSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 1.5f;
                        gameObject.AddComponent<RagdollSpawnPointComponent>().Init();
                        break;
                    }

                case ToolGunMode.SportShootingTarget:
                case ToolGunMode.DboyShootingTarget:
                case ToolGunMode.BinaryShootingTarget:
                    {
                        gameObject.AddComponent<ShootingTargetComponent>();
                        break;
                    }

                case ToolGunMode.LightController:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<LightControllerComponent>().Init();
                        break;
                    }
            }

            SpawnedObjects.Add(gameObject.GetComponent<MapEditorObject>());
            NetworkServer.Spawn(gameObject);
        }

        /// <summary>
        /// Spawns a copy of selected object by a ToolGun.
        /// </summary>
        /// <param name="position">Position of spawned property object.</param>
        /// <param name="prefab">The <see cref="GameObject"/> from which the copy will be spawned.</param>
        public static void SpawnPropertyObject(Vector3 position, GameObject prefab)
        {
            GameObject gameObject = Object.Instantiate(prefab, position, prefab.transform.rotation);
            gameObject.name = gameObject.name.Replace("(Clone)(Clone)(Clone)", "(Clone)");

            SpawnedObjects.Add(gameObject.GetComponent<MapEditorObject>());
            NetworkServer.Spawn(gameObject);

            Log.Debug(gameObject.name, Config.Debug);
        }

        #endregion

        #region Getting Relative Stuff Methods

        /// <summary>
        /// Gets a random <see cref="Room"/> from the <see cref="RoomType"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoomType"/> from which the room should be choosen.</param>
        /// <returns>A random <see cref="Room"/> that has <see cref="Room.Type"/> of the argument.</returns>
        public static Room GetRandomRoom(RoomType type)
        {
            List<Room> validRooms = Map.Rooms.Where(x => x.Type == type).ToList();

            return validRooms[Random.Range(0, validRooms.Count)];
        }

        /// <summary>
        /// Gets a position relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="position">The object position.</param>
        /// <param name="room">The <see cref="Room"/> whose <see cref="Transform"/> will be used.</param>
        /// <returns>Global position relative to the <see cref="Room"/>. If the <paramref name="type"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="position"/> will be retured with no changes.</returns>
        public static Vector3 GetRelativePosition(Vector3 position, Room room)
        {
            if (room.Type == RoomType.Surface)
            {
                return position;
            }
            else
            {
                return room.transform.TransformPoint(position);
            }
        }

        /// <summary>
        /// Gets a rotation relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="rotation">The object rotation.</param>
        /// <param name="room">The <see cref="Room"/> whose <see cref="Transform"/> will be used.</param>
        /// <returns>Global rotation relative to the <see cref="Room"/>. If the <paramref name="roomType"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="rotation"/> will be retured with no changes.</returns>
        public static Quaternion GetRelativeRotation(Vector3 rotation, Room room)
        {
            if (rotation.x == -1f)
                rotation.x = Random.Range(0f, 360f);

            if (rotation.y == -1f)
                rotation.y = Random.Range(0f, 360f);

            if (rotation.z == -1f)
                rotation.z = Random.Range(0f, 360f);

            if (room.Type == RoomType.Surface)
            {
                return Quaternion.Euler(rotation);
            }
            else
            {
                return room.transform.rotation * Quaternion.Euler(rotation);
            }
        }

        #endregion

        #region Spawning Indicators

        /// <summary>
        /// Spawns a pickup indicator used for showing where ItemSpawnPoint is.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="GameObject"/> of the ItemSpawnPoint which indicator should indicate.</param>
        public static void SpawnPickupIndicator(GameObject itemSpawnPoint) => SpawnPickupIndicator(itemSpawnPoint.transform.position, itemSpawnPoint.transform.rotation, itemSpawnPoint.GetComponent<ItemSpawnPointComponent>().ItemName, itemSpawnPoint);

        /// <inheritdoc cref="SpawnPickupIndicator(GameObject)"/>
        public static void SpawnPickupIndicator(Vector3 position, Quaternion rotation, string name, GameObject callingItemSpawnPointObject)
        {
            if (Indicators.Values.Contains(callingItemSpawnPointObject))
            {
                GameObject pickupIndicator = Indicators.First(x => x.Value == callingItemSpawnPointObject).Key;

                pickupIndicator.gameObject.transform.position = position;
                pickupIndicator.gameObject.transform.rotation = rotation;
                return;
            }

            ItemType parsedItem;

            if (CustomItem.TryGet(name, out CustomItem custom))
            {
                parsedItem = custom.Type;
            }
            else
            {
                parsedItem = (ItemType)Enum.Parse(typeof(ItemType), name, true);
            }

            Pickup pickup = new Item(parsedItem).Spawn(position + (Vector3.up * 0.1f), rotation);
            pickup.Locked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;

            NetworkServer.UnSpawn(pickupGameObject);

            pickupGameObject.GetComponent<Rigidbody>().isKinematic = true;

            if (parsedItem.IsWeapon())
                pickupGameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            pickupGameObject.AddComponent<ItemSpiningComponent>();

            Indicators.Add(pickupGameObject, callingItemSpawnPointObject);

            NetworkServer.Spawn(pickupGameObject);
        }

        /// <summary>
        /// Spawns a dummy (NPC) indicator used for showning where PlayerSpawnPoint and RagdollSpawnPoint are.
        /// </summary>
        /// <param name="callingGameObject">The <see cref="GameObject"/> of PlayerSpawnPoint or RagdollSpawnPoint which indicator should indicate.</param>
        public static void SpawnDummyIndicator(GameObject callingGameObject)
        {
            Log.Error(callingGameObject.name);

            if (callingGameObject.name == "PlayerSpawnPointObject(Clone)")
            {
                SpawnDummyIndicator(callingGameObject.transform.position, callingGameObject.tag.ConvertToRoleType(), callingGameObject);
            }
            else if (callingGameObject.name == "RagdollSpawnPointObject(Clone)")
            {
                SpawnDummyIndicator(callingGameObject.transform.position, callingGameObject.GetComponent<RagdollSpawnPointComponent>().RagdollRoleType, callingGameObject);
            }
        }

        /// <inheritdoc cref="SpawnDummyIndicator(GameObject)"/>
        public static void SpawnDummyIndicator(Vector3 posistion, RoleType type, GameObject callingGameObject)
        {
            if (Indicators.Values.Contains(callingGameObject))
            {
                ReferenceHub dummyIndicator = Indicators.First(x => x.Value == callingGameObject).Key.GetComponent<ReferenceHub>();

                try
                {
                    dummyIndicator.transform.position = posistion;
                    dummyIndicator.playerMovementSync.OverridePosition(posistion, 0f);
                }
                catch
                {
                    return;
                }

                return;
            }

            GameObject dummyObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);

            dummyObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            if (callingGameObject.name.Contains("Ragdoll"))
                dummyObject.transform.localScale *= -1;

            dummyObject.transform.position = posistion;

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();

            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = type;
            ccm.GodMode = true;

            string dummyNickname;

            switch (type)
            {
                case RoleType.NtfPrivate:
                    dummyNickname = "MTF";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;

                default:
                    dummyNickname = type.ToString();
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();

            if (callingGameObject.name.Contains("Ragdoll"))
            {
                nicknameSync.Network_myNickSync = "RAGDOLL SPAWNPOINT";
                nicknameSync.CustomPlayerInfo = $"{dummyNickname} RAGDOLL\nSPAWN POINT";
            }
            else
            {
                nicknameSync.Network_myNickSync = "PLAYER SPAWNPOINT";
                nicknameSync.CustomPlayerInfo = $"{dummyNickname}\nSPAWN POINT";
            }

            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            NetworkServer.Spawn(dummyObject);
            PlayerManager.players.Add(dummyObject);
            Indicators.Add(dummyObject, callingGameObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.5f, () =>
            {
                dummyObject.AddComponent<DummySpiningComponent>().Hub = rh;
                rh.playerMovementSync.OverridePosition(posistion, 0f);
            });
        }

        #endregion
    }
}