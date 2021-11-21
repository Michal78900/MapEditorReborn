namespace MapEditorReborn
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;
    using Exiled.Loader;
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
    public static partial class Handler
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
                mapEditorObject?.Destroy();
            }

            SpawnedObjects.Clear();

            Log.Debug("Destroyed all map's GameObjects and indicators.", Config.Debug);

            // This is to bring vanilla spawnpoints to their previous state.
            PlayerSpawnPointComponent.VanillaSpawnPointsDisabled = false;

            // This is to remove selected object hint.
            foreach (Player player in Player.List)
            {
                SelectObject(player, null);
            }

            if (map == null)
            {
                Log.Debug("Map is null. Returning...", Config.Debug);
                return;
            }

            foreach (DoorObject door in map.Doors)
            {
                Log.Debug($"Trying to spawn door at {door.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnDoor(door));
            }

            if (map.Doors.Count > 0)
                Log.Debug("All doors have been successfully spawned!", Config.Debug);

            foreach (WorkStationObject workstation in map.WorkStations)
            {
                Log.Debug($"Spawning workstation at {workstation.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnWorkStation(workstation));
            }

            if (map.WorkStations.Count > 0)
                Log.Debug("All workstations have been successfully spawned!", Config.Debug);

            foreach (ItemSpawnPointObject itemSpawnPoint in map.ItemSpawnPoints)
            {
                Log.Debug($"Trying to spawn a item spawn point at {itemSpawnPoint.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnItemSpawnPoint(itemSpawnPoint));
            }

            if (map.ItemSpawnPoints.Count > 0)
                Log.Debug("All item spawn points have been spawned!", Config.Debug);

            foreach (PlayerSpawnPointObject playerSpawnPoint in map.PlayerSpawnPoints)
            {
                Log.Debug($"Trying to spawn a player spawn point at {playerSpawnPoint.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnPlayerSpawnPoint(playerSpawnPoint));
            }

            if (map.PlayerSpawnPoints.Count > 0)
                Log.Debug("All player spawn points have been spawned!", Config.Debug);

            PlayerSpawnPointComponent.VanillaSpawnPointsDisabled = map.RemoveDefaultSpawnPoints;

            foreach (RagdollSpawnPointObject ragdollSpawnPoint in map.RagdollSpawnPoints)
            {
                Log.Debug($"Trying to spawn a ragdoll spawn point at {ragdollSpawnPoint.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnRagdollSpawnPoint(ragdollSpawnPoint));
            }

            if (map.RagdollSpawnPoints.Count > 0)
                Log.Debug("All ragdoll spawn points have been spawned!", Config.Debug);

            foreach (ShootingTargetObject shootingTargetObject in map.ShootingTargetObjects)
            {
                Log.Debug($"Trying to spawn a shooting target at {shootingTargetObject.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnShootingTarget(shootingTargetObject));
            }

            if (map.ShootingTargetObjects.Count > 0)
                Log.Debug("All shooting targets have been spawned!", Config.Debug);

            foreach (LightControllerObject lightControllerObject in map.LightControllerObjects)
            {
                Log.Debug($"Trying to spawn a light controller at {lightControllerObject.RoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnLightController(lightControllerObject));
            }

            if (map.LightControllerObjects.Count > 0)
                Log.Debug("All light controllers have been spawned!", Config.Debug);

            foreach (TeleportObject teleportObject in map.TeleportObjects)
            {
                Log.Debug($"Trying to spawn a teleporter at {teleportObject.EntranceTeleporterRoomType}...", Config.Debug);
                SpawnedObjects.Add(SpawnTeleport(teleportObject));
            }

            if (map.TeleportObjects.Count > 0)
                Log.Debug("All teleporters have been spawned!", Config.Debug);

            foreach (SchematicObject schematicObject in map.SchematicObjects)
            {
                Log.Debug($"Trying to spawn a schematic named \"{schematicObject.SchematicName}\" at {schematicObject.RoomType}...", Config.Debug);

                MapEditorObject schematic = SpawnSchematic(schematicObject);

                if (schematic == null)
                {
                    Log.Warn($"The schematic with \"{schematicObject.SchematicName}\" name does not exist or has an invalid name. Skipping...");
                    continue;
                }

                SpawnedObjects.Add(schematic);
            }

            if (map.SchematicObjects.Count > 0)
                Log.Debug("All schematics have been spawned!", Config.Debug);

            Log.Debug("All GameObject have been spawned and the MapSchematic has been fully loaded!", Config.Debug);
        }

        /// <summary>
        /// Saves the map to a file.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public static void SaveMap(string name)
        {
            Log.Debug("Trying to save the map...", Config.Debug);

            MapSchematic map = GetMapByName(name);

            if (map == null)
            {
                map = new MapSchematic(name);
            }
            else
            {
                map.CleanupAll();
            }

            Log.Debug($"Map name set to \"{map.Name}\"", Config.Debug);

            foreach (MapEditorObject spawnedObject in SpawnedObjects)
            {
                if (spawnedObject is IndicatorObjectComponent)
                    continue;

                Log.Debug($"Trying to save GameObject at {spawnedObject.transform.position}...", Config.Debug);

                switch (spawnedObject)
                {
                    case DoorObjectComponent door:
                        {
                            door.Base.Position = door.RelativePosition;
                            door.Base.Rotation = door.RelativeRotation;
                            door.Base.Scale = door.Scale;
                            door.Base.RoomType = door.RoomType;

                            map.Doors.Add(door.Base);

                            break;
                        }

                    case WorkStationObjectComponent workStation:
                        {
                            workStation.Base.Position = workStation.RelativePosition;
                            workStation.Base.Rotation = workStation.RelativeRotation;
                            workStation.Base.Scale = workStation.Scale;
                            workStation.Base.RoomType = workStation.RoomType;

                            map.WorkStations.Add(workStation.Base);

                            break;
                        }

                    case PlayerSpawnPointComponent playerspawnPoint:
                        {
                            playerspawnPoint.Base.Position = playerspawnPoint.RelativePosition;
                            playerspawnPoint.Base.RoomType = playerspawnPoint.RoomType;

                            map.PlayerSpawnPoints.Add(playerspawnPoint.Base);

                            break;
                        }

                    case ItemSpawnPointComponent itemSpawnPoint:
                        {
                            itemSpawnPoint.Base.Position = itemSpawnPoint.RelativePosition;
                            itemSpawnPoint.Base.Rotation = itemSpawnPoint.RelativeRotation;
                            itemSpawnPoint.Base.Scale = itemSpawnPoint.Scale;
                            itemSpawnPoint.Base.RoomType = itemSpawnPoint.RoomType;

                            map.ItemSpawnPoints.Add(itemSpawnPoint.Base);

                            break;
                        }

                    case RagdollSpawnPointComponent ragdollSpawnPoint:
                        {
                            ragdollSpawnPoint.Base.Position = ragdollSpawnPoint.RelativePosition;
                            ragdollSpawnPoint.Base.Rotation = ragdollSpawnPoint.RelativeRotation;
                            ragdollSpawnPoint.Base.RoomType = ragdollSpawnPoint.RoomType;

                            map.RagdollSpawnPoints.Add(ragdollSpawnPoint.Base);

                            break;
                        }

                    case ShootingTargetComponent shootingTarget:
                        {
                            shootingTarget.Base.Position = shootingTarget.RelativePosition;
                            shootingTarget.Base.Rotation = shootingTarget.RelativeRotation;
                            shootingTarget.Base.Scale = shootingTarget.Scale;
                            shootingTarget.Base.RoomType = shootingTarget.RoomType;

                            map.ShootingTargetObjects.Add(shootingTarget.Base);

                            break;
                        }

                    case LightControllerComponent lightController:
                        {
                            map.LightControllerObjects.Add(lightController.Base);

                            break;
                        }

                    case TeleportControllerComponent teleportController:
                        {
                            teleportController.Base.EntranceTeleporterPosition = teleportController.EntranceTeleport.RelativePosition;
                            teleportController.Base.EntranceTeleporterScale = teleportController.EntranceTeleport.Scale;
                            teleportController.Base.EntranceTeleporterRoomType = teleportController.EntranceTeleport.RoomType;

                            teleportController.Base.ExitTeleporterPosition = teleportController.ExitTeleport.RelativePosition;
                            teleportController.Base.ExitTeleporterScale = teleportController.ExitTeleport.Scale;
                            teleportController.Base.ExitTeleporterRoomType = teleportController.ExitTeleport.RoomType;

                            map.TeleportObjects.Add(teleportController.Base);

                            break;
                        }

                    case SchematicObjectComponent schematicObject:
                        {
                            schematicObject.Base.Position = schematicObject.RelativePosition;
                            schematicObject.Base.Rotation = schematicObject.RelativeRotation;
                            schematicObject.Base.Scale = schematicObject.Scale;
                            schematicObject.Base.RoomType = schematicObject.RoomType;

                            map.SchematicObjects.Add(schematicObject.Base);

                            break;
                        }
                }
            }

            string path = Path.Combine(MapEditorReborn.MapsDir, $"{map.Name}.yml");

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
            if (mapName == CurrentLoadedMap?.Name)
                return CurrentLoadedMap;

            string path = Path.Combine(MapEditorReborn.MapsDir, $"{mapName}.yml");

            if (!File.Exists(path))
                return null;

            MapSchematic map = Loader.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));
            map.Name = mapName;

            return map;
        }

        /// <summary>
        /// Gets the <see cref="SaveDataObjectList"/> by it's name.
        /// </summary>
        /// <param name="schematicName">The name of the map.</param>
        /// <returns><see cref="SaveDataObjectList"/> if the file with the schematic was found, otherwise <see langword="null"/>.</returns>
        public static SaveDataObjectList GetSchematicDataByName(string schematicName)
        {
            string path = Path.Combine(MapEditorReborn.SchematicsDir, $"{schematicName}.json");

            if (!File.Exists(path))
                return null;

            return Utf8Json.JsonSerializer.Deserialize<SaveDataObjectList>(File.ReadAllText(path));
        }

        #endregion

        #region Spawning Objects Methods

        /// <summary>
        /// Spawns a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorObject"/> which is used to spawn a door.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnDoor(DoorObject door, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(door.RoomType);
            GameObject gameObject = Object.Instantiate(door.DoorType.GetDoorObjectByType(), forcedPosition ?? GetRelativePosition(door.Position, room), forcedRotation ?? GetRelativeRotation(door.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? door.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(door.Rotation);

            return gameObject.AddComponent<DoorObjectComponent>().Init(door);
        }

        /// <summary>
        /// Spawns a workstation.
        /// </summary>
        /// <param name="workStation">The <see cref="WorkStationObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnWorkStation(WorkStationObject workStation, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(workStation.RoomType);
            GameObject gameObject = Object.Instantiate(WorkstationObj, forcedPosition ?? GetRelativePosition(workStation.Position, room), forcedRotation ?? GetRelativeRotation(workStation.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? workStation.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(workStation.Rotation);

            return gameObject.AddComponent<WorkStationObjectComponent>().Init(workStation);
        }

        /// <summary>
        /// Spawns a ItemSpawnPoint.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnItemSpawnPoint(ItemSpawnPointObject itemSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(itemSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(ItemSpawnPointObj, forcedPosition ?? GetRelativePosition(itemSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(itemSpawnPoint.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? itemSpawnPoint.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(itemSpawnPoint.Rotation);

            return gameObject.AddComponent<ItemSpawnPointComponent>().Init(itemSpawnPoint);
        }

        /// <summary>
        /// Spawns a PlayerSpawnPoint.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnPlayerSpawnPoint(PlayerSpawnPointObject playerSpawnPoint, Vector3? forcedPosition = null)
        {
            Room room = GetRandomRoom(playerSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(PlayerSpawnPointObj, forcedPosition ?? GetRelativePosition(playerSpawnPoint.Position, room), Quaternion.identity);
            // gameObject.tag = playerSpawnPoint.RoleType.ConvertToSpawnPointTag();

            // gameObject.AddComponent<ObjectRotationComponent>().Init(gameObject.transform.eulerAngles);

            return gameObject.AddComponent<PlayerSpawnPointComponent>().Init(playerSpawnPoint);
        }

        /// <summary>
        /// Spawns a RagdollSpawnPoint.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnRagdollSpawnPoint(RagdollSpawnPointObject ragdollSpawnPoint, Vector3? forcedPosition = null, Quaternion? forcedRotation = null)
        {
            Room room = GetRandomRoom(ragdollSpawnPoint.RoomType);
            GameObject gameObject = Object.Instantiate(RagdollSpawnPointObj, forcedPosition ?? GetRelativePosition(ragdollSpawnPoint.Position, room), forcedRotation ?? GetRelativeRotation(ragdollSpawnPoint.Rotation, room));

            gameObject.AddComponent<ObjectRotationComponent>().Init(ragdollSpawnPoint.Rotation);

            return gameObject.AddComponent<RagdollSpawnPointComponent>().Init(ragdollSpawnPoint);
        }

        /// <summary>
        /// Spawns a ShootingTarget.
        /// </summary>
        /// <param name="shootingTarget">The <see cref="ShootingTargetObject"/> to spawn.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnShootingTarget(ShootingTargetObject shootingTarget, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            Room room = GetRandomRoom(shootingTarget.RoomType);
            GameObject gameObject = Object.Instantiate(shootingTarget.TargetType.GetShootingTargetObjectByType(), forcedPosition ?? GetRelativePosition(shootingTarget.Position, room), forcedRotation ?? GetRelativeRotation(shootingTarget.Rotation, room));
            gameObject.transform.localScale = forcedScale ?? shootingTarget.Scale;

            gameObject.AddComponent<ObjectRotationComponent>().Init(shootingTarget.Rotation);

            return gameObject.AddComponent<ShootingTargetComponent>().Init(shootingTarget);
        }

        /// <summary>
        /// Spawns a LightController.
        /// </summary>
        /// <param name="lightController">The <see cref="LightControllerObject"/> to spawn.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnLightController(LightControllerObject lightController) => Object.Instantiate(LightControllerObj).AddComponent<LightControllerComponent>().Init(lightController);

        /// <summary>
        /// Spawns a Teleporter.
        /// </summary>
        /// <param name="teleport">The <see cref="TeleportObject"/> to spawn.</param>
        /// <returns>Spawned <see cref="MapEditorObject"/>.</returns>
        public static MapEditorObject SpawnTeleport(TeleportObject teleport) => Object.Instantiate(TeleporterObj).AddComponent<TeleportControllerComponent>().Init(teleport);

        /// <summary>
        /// Spawns a Schematic.
        /// </summary>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> to spawn.</param>
        /// <param name="data">The <see cref="SaveDataObjectList"/> data which contains info about schematic's building blocks.</param>
        /// <param name="forcedPosition">Used to force exact object position.</param>
        /// <param name="forcedRotation">Used to force exact object rotation.</param>
        /// <param name="forcedScale">Used to force exact object scale.</param>
        /// <returns>Spawned <see cref="SchematicObject"/>.</returns>
        public static MapEditorObject SpawnSchematic(SchematicObject schematicObject, SaveDataObjectList data = null, Vector3? forcedPosition = null, Quaternion? forcedRotation = null, Vector3? forcedScale = null)
        {
            if (data == null)
                data = GetSchematicDataByName(schematicObject.SchematicName);

            if (data == null)
                return null;

            Room room = null;

            if (schematicObject.RoomType != RoomType.Unknown)
                room = GetRandomRoom(schematicObject.RoomType);

            GameObject gameObject = new GameObject($"CustomSchematic-{schematicObject.SchematicName}");
            gameObject.transform.position = forcedPosition ?? GetRelativePosition(schematicObject.Position, room);
            gameObject.transform.rotation = forcedRotation ?? GetRelativeRotation(schematicObject.Rotation, room);
            gameObject.transform.localScale = forcedScale ?? schematicObject.Scale;

            return gameObject.AddComponent<SchematicObjectComponent>().Init(schematicObject, data);
        }

        /// <summary>
        /// Spawns a copy of selected object by a ToolGun.
        /// </summary>
        /// <param name="position">Position of spawned property object.</param>
        /// <param name="prefab">The <see cref="GameObject"/> from which the copy will be spawned.</param>
        public static void SpawnPropertyObject(Vector3 position, MapEditorObject prefab)
        {
            Quaternion rotation = prefab.transform.rotation;
            Vector3 scale = prefab.transform.localScale;

            switch (prefab)
            {
                case DoorObjectComponent door:
                    {
                        SpawnedObjects.Add(SpawnDoor(new DoorObject().CopyProperties(door.Base), position, rotation, scale));
                        break;
                    }

                case WorkStationObjectComponent workStation:
                    {
                        SpawnedObjects.Add(SpawnWorkStation(new WorkStationObject().CopyProperties(workStation.Base), position, rotation, scale));
                        break;
                    }

                case ItemSpawnPointComponent itemSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnItemSpawnPoint(new ItemSpawnPointObject().CopyProperties(itemSpawnPoint.Base), position, rotation, scale));
                        break;
                    }

                case PlayerSpawnPointComponent playerSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnPlayerSpawnPoint(new PlayerSpawnPointObject().CopyProperties(playerSpawnPoint.Base), position));
                        break;
                    }

                case RagdollSpawnPointComponent ragdollSpawnPoint:
                    {
                        SpawnedObjects.Add(SpawnRagdollSpawnPoint(new RagdollSpawnPointObject().CopyProperties(ragdollSpawnPoint.Base), position, rotation));
                        break;
                    }

                case ShootingTargetComponent shootingTarget:
                    {
                        SpawnedObjects.Add(SpawnShootingTarget(new ShootingTargetObject().CopyProperties(shootingTarget.Base), position, rotation, scale));
                        break;
                    }

                case SchematicObjectComponent schematic:
                    {
                        SpawnedObjects.Add(SpawnSchematic(new SchematicObject().CopyProperties(schematic.Base), null, position + Vector3.up, rotation, scale));
                        break;
                    }
            }

            if (Config.ShowIndicatorOnSpawn)
                SpawnedObjects.Last().UpdateIndicator();
        }

        #endregion

        #region ToolGun Methods

        /// <summary>
        /// Spawns a general <see cref="MapEditorObject"/>.
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
                        gameObject.AddComponent<DoorObjectComponent>().Init(new DoorObject());
                        break;
                    }

                case ToolGunMode.WorkStation:
                    {
                        gameObject.AddComponent<WorkStationObjectComponent>().Init(new WorkStationObject());
                        break;
                    }

                case ToolGunMode.ItemSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.1f;
                        gameObject.AddComponent<ItemSpawnPointComponent>().Init(new ItemSpawnPointObject());
                        break;
                    }

                case ToolGunMode.PlayerSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<PlayerSpawnPointComponent>().Init(new PlayerSpawnPointObject());
                        break;
                    }

                case ToolGunMode.RagdollSpawnPoint:
                    {
                        gameObject.transform.position += Vector3.up * 1.5f;
                        gameObject.AddComponent<RagdollSpawnPointComponent>().Init(new RagdollSpawnPointObject());
                        break;
                    }

                case ToolGunMode.SportShootingTarget:
                case ToolGunMode.DboyShootingTarget:
                case ToolGunMode.BinaryShootingTarget:
                    {
                        gameObject.AddComponent<ShootingTargetComponent>().Init(new ShootingTargetObject());
                        break;
                    }

                case ToolGunMode.LightController:
                    {
                        gameObject.transform.position += Vector3.up * 0.25f;
                        gameObject.AddComponent<LightControllerComponent>().Init(new LightControllerObject());
                        break;
                    }

                case ToolGunMode.Teleporter:
                    {
                        gameObject.transform.position += Vector3.up;
                        gameObject.AddComponent<TeleportControllerComponent>().Init(new TeleportObject());
                        break;
                    }
            }

            MapEditorObject mapObject = gameObject.GetComponent<MapEditorObject>();
            SpawnedObjects.Add(mapObject);

            if (Config.ShowIndicatorOnSpawn)
                Timing.CallDelayed(0.1f, () => mapObject.UpdateIndicator());
        }

        /// <summary>
        /// Tries getting <see cref="MapEditorObject"/> through Raycasting.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that is used in raycasting.</param>
        /// <param name="mapObject">The found <see cref="MapEditorObject"/>. May be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="MapEditorObject"/> was found, otherwise <see langword="false"/>.</returns>
        public static bool TryGetMapObject(Player player, out MapEditorObject mapObject)
        {
            Vector3 forward = player.CameraTransform.forward;
            if (Physics.Raycast(player.CameraTransform.position + forward, forward, out RaycastHit hit, 100f))
            {
                mapObject = hit.collider.GetComponentInParent<MapEditorObject>();

                IndicatorObjectComponent indicator = mapObject?.GetComponent<IndicatorObjectComponent>();
                if (indicator != null)
                {
                    mapObject = indicator.AttachedMapEditorObject;
                    return true;
                }

                SchematicBlockComponent schematicBlock = mapObject?.GetComponent<SchematicBlockComponent>();
                if (schematicBlock != null)
                {
                    mapObject = schematicBlock.AttachedSchematic;
                    return true;
                }

                if (mapObject == null)
                {
                    foreach (Vector3 pos in LightControllerComponent.FlickerableLightsPositions)
                    {
                        float sqrDistance = (pos - hit.point).sqrMagnitude;

                        if (sqrDistance <= 2.5f)
                        {
                            mapObject = SpawnedObjects.FirstOrDefault(x => x is LightControllerComponent lightComp && lightComp.RoomType == Map.FindParentRoom(hit.collider.gameObject).Type);
                            break;
                        }
                    }
                }

                return mapObject != null;
            }

            mapObject = null;
            return false;
        }

        /// <summary>
        /// Copies the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that copies the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to copy.</param>
        public static void CopyObject(Player player, MapEditorObject mapObject)
        {
            if (mapObject != null && SpawnedObjects.Contains(mapObject))
            {
                if (!player.SessionVariables.ContainsKey(CopiedObjectSessionVarName))
                {
                    player.SessionVariables.Add(CopiedObjectSessionVarName, mapObject);
                }
                else
                {
                    player.SessionVariables[CopiedObjectSessionVarName] = mapObject;
                }

                player.ShowHint("Object properties have been copied to the ToolGun.");
            }
            else if (player.SessionVariables.ContainsKey(CopiedObjectSessionVarName))
            {
                player.SessionVariables.Remove(CopiedObjectSessionVarName);
                player.ShowHint("ToolGun has been reseted to default settings.");
            }
        }

        /// <summary>
        /// Selects the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that selects the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to select.</param>
        public static void SelectObject(Player player, MapEditorObject mapObject)
        {
            if (mapObject != null && (SpawnedObjects.Contains(mapObject) || mapObject is TeleportComponent))
            {
                player.ShowGameObjectHint(mapObject);

                if (!player.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
                {
                    player.SessionVariables.Add(SelectedObjectSessionVarName, mapObject);
                }
                else
                {
                    player.SessionVariables[SelectedObjectSessionVarName] = mapObject;
                }
            }
            else if (player.SessionVariables.ContainsKey(SelectedObjectSessionVarName))
            {
                player.SessionVariables.Remove(SelectedObjectSessionVarName);
                player.ShowHint("Object has been unselected");
            }
        }

        /// <summary>
        /// Deletes the <see cref="MapEditorObject"/>.
        /// </summary>
        /// <param name="player">The player that deletes the object.</param>
        /// <param name="mapObject">The <see cref="MapEditorObject"/> to delete.</param>
        public static void DeleteObject(Player player, MapEditorObject mapObject)
        {
            MapEditorObject indicator = mapObject.AttachedIndicator;
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                indicator.Destroy();

                if (mapObject is TeleportComponent teleport)
                {
                    indicator = teleport.IsEntrance ? teleport.Controller.ExitTeleport.AttachedIndicator : teleport.Controller.EntranceTeleport.AttachedIndicator;

                    SpawnedObjects.Remove(indicator);
                    indicator.Destroy();
                }
            }

            if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject selectedObject) && selectedObject == mapObject)
            {
                player.SessionVariables.Remove(SelectedObjectSessionVarName);
                player.ShowHint(string.Empty, 0.1f);
            }

            if (mapObject.transform.parent != null)
                mapObject = mapObject.transform.parent.GetComponent<MapEditorObject>();

            player.RemoteAdminMessage(mapObject.ToString());

            SpawnedObjects.Remove(mapObject);
            mapObject.Destroy();
        }

        public static string GetToolGunModeText(Player player, bool isAiming, bool flashlightEnabled) => isAiming ? flashlightEnabled ? Translation.ModeSelecting : Translation.ModeCopying : flashlightEnabled ? $"{Translation.ModeCreating}\n<b>({ToolGuns[player.CurrentItem.Serial]})</b>" : Translation.ModeDeleting;

        #endregion

        #region Getting Relative Stuff Methods

        /// <summary>
        /// Gets or sets a random <see cref="Room"/> from the <see cref="RoomType"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoomType"/> from which the room should be choosen.</param>
        /// <returns>A random <see cref="Room"/> that has <see cref="Room.Type"/> of the argument.</returns>
        public static Room GetRandomRoom(RoomType type)
        {
            if (type == RoomType.Unknown)
                return null;

            List<Room> validRooms = Map.Rooms.Where(x => x.Type == type).ToList();

            // return validRooms[Random.Range(0, validRooms.Count)];
            return validRooms.First();
        }

        /// <summary>
        /// Gets or sets a position relative to the <see cref="Room"/>.
        /// </summary>
        /// <param name="position">The object position.</param>
        /// <param name="room">The <see cref="Room"/> whose <see cref="Transform"/> will be used.</param>
        /// <returns>Global position relative to the <see cref="Room"/>. If the <paramref name="type"/> is equal to <see cref="RoomType.Surface"/> the <paramref name="position"/> will be retured with no changes.</returns>
        public static Vector3 GetRelativePosition(Vector3 position, Room room) => room.Type == RoomType.Surface ? position : room.transform.TransformPoint(position);

        /// <summary>
        /// Gets or sets a rotation relative to the <see cref="Room"/>.
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

            if (room == null)
                return Quaternion.Euler(rotation);

            return room.Type == RoomType.Surface ? Quaternion.Euler(rotation) : room.transform.rotation * Quaternion.Euler(rotation);
        }

        #endregion

        #region Spawning Indicators

        /// <summary>
        /// Spawns indicator that indiactes ItemSpawnPoint object.
        /// </summary>
        /// <param name="itemSpawnPoint">The <see cref="ItemSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(ItemSpawnPointComponent itemSpawnPoint, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                indicator.Destroy();
            }

            Vector3 position = itemSpawnPoint.transform.position;
            Quaternion rotation = itemSpawnPoint.transform.rotation;

            ItemType parsedItem;

            if (CustomItem.TryGet(itemSpawnPoint.Base.Item, out CustomItem custom))
            {
                parsedItem = custom.Type;
            }
            else
            {
                parsedItem = (ItemType)Enum.Parse(typeof(ItemType), itemSpawnPoint.Base.Item, true);
            }

            Pickup pickup = new Item(parsedItem).Spawn(position + (Vector3.up * 0.1f), rotation);
            pickup.Locked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;
            NetworkServer.UnSpawn(pickupGameObject);

            pickupGameObject.GetComponent<Rigidbody>().isKinematic = true;

            if (Exiled.API.Extensions.ItemExtensions.IsWeapon(parsedItem))
                pickupGameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            pickupGameObject.AddComponent<ItemSpiningComponent>();

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObjectComponent>().Init(itemSpawnPoint));
            NetworkServer.Spawn(pickupGameObject);
        }

        /// <summary>
        /// Spawns indicator that indiactes PlayerSpawnPoint object.
        /// </summary>
        /// <param name="playerSpawnPoint">The <see cref="PlayerSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(PlayerSpawnPointComponent playerSpawnPoint, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                NetworkServer.Destroy(indicator.gameObject);
            }

            Vector3 position = playerSpawnPoint.transform.position;

            GameObject dummyObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
            dummyObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            dummyObject.transform.position = position;

            RoleType roleType = playerSpawnPoint.tag.ConvertToRoleType();

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();

            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = playerSpawnPoint.tag.ConvertToRoleType();
            ccm.GodMode = true;

            string dummyNickname = roleType.ToString();

            switch (roleType)
            {
                case RoleType.NtfPrivate:
                    dummyNickname = "MTF";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();
            nicknameSync.Network_myNickSync = "PLAYER SPAWNPOINT";
            nicknameSync.CustomPlayerInfo = $"{dummyNickname}\nSPAWN POINT";
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            SpawnedObjects.Add(dummyObject.AddComponent<IndicatorObjectComponent>().Init(playerSpawnPoint));
            NetworkServer.Spawn(dummyObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.1f, () =>
            {
                rh.playerMovementSync.OverridePosition(position, 0f);
            });
        }

        /// <summary>
        /// Spawns indicator that indiactes RagdollSpawnPoint object.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointComponent"/> that is used for spawning the indicator.</param>
        /// <param name="indicator">The <see cref="IndicatorObjectComponent"/> that already exists and may be use to just update the indicator.</param>
        public static void SpawnObjectIndicator(RagdollSpawnPointComponent ragdollSpawnPoint, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                NetworkServer.Destroy(indicator.gameObject);
            }

            Vector3 position = ragdollSpawnPoint.transform.position;

            GameObject dummyObject = Object.Instantiate(LiteNetLib4MirrorNetworkManager.singleton.playerPrefab);
            dummyObject.transform.localScale = new Vector3(-0.2f, -0.2f, -0.2f);
            dummyObject.transform.position = position;

            RoleType roleType = ragdollSpawnPoint.Base.RoleType;

            QueryProcessor processor = dummyObject.GetComponent<QueryProcessor>();
            processor.NetworkPlayerId = QueryProcessor._idIterator++;
            processor._ipAddress = "127.0.0.WAN";

            CharacterClassManager ccm = dummyObject.GetComponent<CharacterClassManager>();
            ccm.CurClass = roleType;
            ccm.GodMode = true;

            string dummyNickname = roleType.ToString();

            switch (roleType)
            {
                case RoleType.NtfPrivate:
                    dummyNickname = "MTF";
                    break;

                case RoleType.Scp93953:
                    dummyNickname = "SCP939";
                    break;
            }

            NicknameSync nicknameSync = dummyObject.GetComponent<NicknameSync>();
            nicknameSync.Network_myNickSync = "RAGDOLL SPAWNPOINT";
            nicknameSync.CustomPlayerInfo = $"{dummyNickname} RAGDOLL\nSPAWN POINT";
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Nickname;
            nicknameSync.ShownPlayerInfo &= ~PlayerInfoArea.Role;

            SpawnedObjects.Add(dummyObject.AddComponent<IndicatorObjectComponent>().Init(ragdollSpawnPoint));
            NetworkServer.Spawn(dummyObject);

            ReferenceHub rh = dummyObject.GetComponent<ReferenceHub>();
            Timing.CallDelayed(0.1f, () =>
            {
                rh.playerMovementSync.OverridePosition(position, 0f);
            });
        }

        public static void SpawnObjectIndicator(TeleportComponent teleport, bool isEntrance, IndicatorObjectComponent indicator = null)
        {
            if (indicator != null)
            {
                SpawnedObjects.Remove(indicator);
                NetworkServer.Destroy(indicator.gameObject);
            }

            Vector3 position = teleport.transform.position;

            Pickup pickup = new Item(isEntrance ? ItemType.KeycardZoneManager : ItemType.KeycardFacilityManager).Spawn(position);
            pickup.Locked = true;

            GameObject pickupGameObject = pickup.Base.gameObject;
            NetworkServer.UnSpawn(pickupGameObject);

            pickupGameObject.GetComponent<Rigidbody>().isKinematic = true;

            pickupGameObject.transform.localScale = Vector3.Scale(new Vector3(4.5f, 130f, 7.5f), teleport.Scale);

            SpawnedObjects.Add(pickupGameObject.AddComponent<IndicatorObjectComponent>().Init(teleport));
            NetworkServer.Spawn(pickupGameObject);
        }

        #endregion
    }
}