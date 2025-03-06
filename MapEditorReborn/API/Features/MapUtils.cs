// -----------------------------------------------------------------------
// <copyright file="MapUtils.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Loader;
    using MEC;
    using NorthwoodLib.Pools;
    using Objects;
    using Objects.Vanilla;
    using Serializable;
    using Serializable.Vanilla;
    using Utf8Json;
    using static API;
    using Config = global::MapEditorReborn.Configs.Config;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A class which contains a few useful methods to interact with maps.
    /// </summary>
    public static class MapUtils
    {
        /// <summary>
        /// Loads the <see cref="Serializable.MapSchematic"/> map.
        /// It also may be used for reloading the map.
        /// </summary>
        /// <param name="mapName">The name of the map to load.</param>
        public static void LoadMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
            {
                LoadMap((MapSchematic)null);
                return;
            }

            MapSchematic mapSchematic = GetMapByName(mapName);
            if (mapSchematic is null)
            {
                Log.Error($"Map {mapName} does not exist!");
                return;
            }

            LoadMap(mapSchematic);
        }

        /// <summary>
        /// Loads the <see cref="Serializable.MapSchematic"/> map.
        /// It also may be used for reloading the map.
        /// </summary>
        /// <param name="map"><see cref="Serializable.MapSchematic"/> to load.</param>
        public static void LoadMap(MapSchematic? map)
        {
            if (map is not null && !map.IsValid)
            {
                Log.Warn($"{map.Name} couldn't be loaded because one of it's object is in RoomType that didn't spawn this round!");
                return;
            }

            API.MapSchematic = map;

            Log.Debug(map is not null ? "Trying to load the map..." : "Trying to unload the map..");

            foreach (MapEditorObject mapEditorObject in SpawnedObjects)
            {
                try
                {
                    mapEditorObject.Destroy();
                }
                catch (Exception)
                {
                    // Ignored
                }
            }

            SpawnedObjects.Clear();

            Log.Debug("Destroyed all map's GameObjects and indicators.");

            // This is to bring vanilla spawnpoints to their previous state.
            // PlayerSpawnPointObject.VanillaSpawnPointsDisabled = false;

            // This is to remove selected object hint.
            foreach (Player player in Player.List)
                ToolGunHandler.SelectObject(player, null);

            // Remove custom properties from vanilla doors
            VanillaDoorObject.UnSetAllDoors();

            // Unregister vanilla tesla events
            VanillaTeslaHandler.UnRegisterEvents();

            if (map == null)
            {
                Log.Debug("Map is null. Returning...");
                return;
            }

            Log.Debug("Setting custom properties for vanilla tesla doors...");
            foreach (KeyValuePair<string, VanillaDoorSerializable> vanillaDoor in map.VanillaDoors)
                VanillaDoorObject.SetDoor(vanillaDoor.Key, vanillaDoor.Value);

            Log.Debug("Setting custom properties for vanilla tesla gates...");
            VanillaTeslaHandler.RegisterEvents();

            foreach (DoorSerializable door in map.Doors)
            {
                Log.Debug($"Trying to spawn door at {door.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnDoor(door));
            }

            if (map.Doors.Count > 0)
                Log.Debug("All doors have been successfully spawned!");

            foreach (WorkstationSerializable workstation in map.WorkStations)
            {
                Log.Debug($"Spawning workstation at {workstation.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnWorkstation(workstation));
            }

            if (map.WorkStations.Count > 0)
                Log.Debug("All workstations have been successfully spawned!");

            foreach (ItemSpawnPointSerializable itemSpawnPoint in map.ItemSpawnPoints)
            {
                Log.Debug($"Trying to spawn a item spawn point at {itemSpawnPoint.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnItemSpawnPoint(itemSpawnPoint));
            }

            if (map.ItemSpawnPoints.Count > 0)
                Log.Debug("All item spawn points have been spawned!");

            foreach (PlayerSpawnPointSerializable playerSpawnPoint in map.PlayerSpawnPoints)
            {
                Log.Debug($"Trying to spawn a player spawn point at {playerSpawnPoint.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnPlayerSpawnPoint(playerSpawnPoint));
            }

            if (map.PlayerSpawnPoints.Count > 0)
                Log.Debug("All player spawn points have been spawned!");

            // PlayerSpawnPointObject.VanillaSpawnPointsDisabled = map.RemoveDefaultSpawnPoints;

            foreach (RagdollSpawnPointSerializable ragdollSpawnPoint in map.RagdollSpawnPoints)
            {
                Log.Debug($"Trying to spawn a ragdoll spawn point at {ragdollSpawnPoint.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnRagdollSpawnPoint(ragdollSpawnPoint));
            }

            if (map.RagdollSpawnPoints.Count > 0)
                Log.Debug("All ragdoll spawn points have been spawned!");

            foreach (ShootingTargetSerializable shootingTargetObject in map.ShootingTargets)
            {
                Log.Debug($"Trying to spawn a shooting target at {shootingTargetObject.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnShootingTarget(shootingTargetObject));
            }

            if (map.ShootingTargets.Count > 0)
                Log.Debug("All shooting targets have been spawned!");

            foreach (PrimitiveSerializable primitiveObject in map.Primitives)
            {
                SpawnedObjects.Add(ObjectSpawner.SpawnPrimitive(primitiveObject));
            }

            foreach (RoomLightSerializable lightControllerObject in map.RoomLights)
            {
                Log.Debug($"Trying to spawn a light controller at {lightControllerObject.RoomType}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnRoomLight(lightControllerObject));
            }

            if (map.RoomLights.Count > 0)
                Log.Debug("All light controllers have been spawned!");

            foreach (LightSourceSerializable lightSourceObject in map.LightSources)
            {
                SpawnedObjects.Add(ObjectSpawner.SpawnLightSource(lightSourceObject));
            }

            foreach (SerializableTeleport teleportObject in map.Teleports)
            {
                Log.Debug($"Trying to spawn a teleporter at {teleportObject.Position}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnTeleport(teleportObject));
            }

            if (map.Teleports.Count > 0)
                Log.Debug("All teleporters have been spawned!");

            foreach (LockerSerializable lockerSerializable in map.Lockers)
            {
                Log.Debug($"Trying to spawn a locker at {lockerSerializable.Position}...");
                SpawnedObjects.Add(ObjectSpawner.SpawnLocker(lockerSerializable));
            }

            if (map.Lockers.Count > 0)
                Log.Debug("All lockers have been spawned!");

            foreach (SchematicSerializable schematicObject in map.Schematics)
            {
                Log.Debug($"Trying to spawn a schematic named \"{schematicObject.SchematicName}\" at {schematicObject.RoomType}... ({schematicObject.Position.x}, {schematicObject.Position.y}, {schematicObject.Position.z})");
                MapEditorObject schematic = ObjectSpawner.SpawnSchematic(schematicObject, null, null, null, null);

                if (schematic == null)
                {
                    Log.Warn($"The schematic with \"{schematicObject.SchematicName}\" name does not exist or has an invalid name. Skipping...");
                    continue;
                }

                SpawnedObjects.Add(schematic);
            }

            if (map.Schematics.Count > 0)
                Log.Debug("All schematics have been spawned!");

            Log.Debug("All GameObject have been spawned and the MapSchematic has been fully loaded!");
        }

        /// <summary>
        /// Saves the map to a file.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public static void SaveMap(string name)
        {
            Log.Debug("Trying to save the map...");
            MapSchematic? map = GetMapByName(name);

            if (map == null)
            {
                map = new MapSchematic(name);
            }
            else
            {
                map.CleanupAll();
            }

            Log.Debug($"Map name set to \"{map.Name}\"");
            foreach (MapEditorObject spawnedObject in SpawnedObjects)
            {
                try
                {
                    if (spawnedObject is IndicatorObject)
                        continue;

                    Log.Debug($"Trying to save GameObject at {spawnedObject.transform.position}...");

                    switch (spawnedObject)
                    {
                        case DoorObject door:
                            {
                                door.Base.Position = door.RelativePosition;
                                door.Base.Rotation = door.RelativeRotation;
                                door.Base.Scale = door.Scale;
                                door.Base.RoomType = door.RoomType;

                                map.Doors.Add(door.Base);

                                break;
                            }

                        case WorkstationObject workStation:
                            {
                                workStation.Base.Position = workStation.RelativePosition;
                                workStation.Base.Rotation = workStation.RelativeRotation;
                                workStation.Base.Scale = workStation.Scale;
                                workStation.Base.RoomType = workStation.RoomType;

                                map.WorkStations.Add(workStation.Base);

                                break;
                            }

                        case PlayerSpawnPointObject playerSpawnPoint:
                            {
                                playerSpawnPoint.Base.Position = playerSpawnPoint.RelativePosition;
                                playerSpawnPoint.Base.RoomType = playerSpawnPoint.RoomType;

                                map.PlayerSpawnPoints.Add(playerSpawnPoint.Base);

                                break;
                            }

                        case ItemSpawnPointObject itemSpawnPoint:
                            {
                                itemSpawnPoint.Base.Position = itemSpawnPoint.RelativePosition;
                                itemSpawnPoint.Base.Rotation = itemSpawnPoint.RelativeRotation;
                                itemSpawnPoint.Base.Scale = itemSpawnPoint.Scale;
                                itemSpawnPoint.Base.RoomType = itemSpawnPoint.RoomType;

                                map.ItemSpawnPoints.Add(itemSpawnPoint.Base);

                                break;
                            }

                        case RagdollSpawnPointObject ragdollSpawnPoint:
                            {
                                ragdollSpawnPoint.Base.Position = ragdollSpawnPoint.RelativePosition;
                                ragdollSpawnPoint.Base.Rotation = ragdollSpawnPoint.RelativeRotation;
                                ragdollSpawnPoint.Base.RoomType = ragdollSpawnPoint.RoomType;

                                map.RagdollSpawnPoints.Add(ragdollSpawnPoint.Base);

                                break;
                            }

                        case ShootingTargetObject shootingTarget:
                            {
                                shootingTarget.Base.Position = shootingTarget.RelativePosition;
                                shootingTarget.Base.Rotation = shootingTarget.RelativeRotation;
                                shootingTarget.Base.Scale = shootingTarget.Scale;
                                shootingTarget.Base.RoomType = shootingTarget.RoomType;

                                map.ShootingTargets.Add(shootingTarget.Base);

                                break;
                            }

                        case PrimitiveObject primitive:
                            {
                                primitive.Base.Position = primitive.RelativePosition;
                                primitive.Base.Rotation = primitive.RelativeRotation;
                                primitive.Base.RoomType = primitive.RoomType;
                                primitive.Base.Scale = primitive.Scale;

                                map.Primitives.Add(primitive.Base);

                                break;
                            }

                        case RoomLightObject lightController:
                            {
                                map.RoomLights.Add(lightController.Base);

                                break;
                            }

                        case LightSourceObject lightSource:
                            {
                                lightSource.Base.Position = lightSource.RelativePosition;
                                lightSource.Base.RoomType = lightSource.RoomType;

                                map.LightSources.Add(lightSource.Base);

                                break;
                            }

                        case TeleportObject teleportController:
                            {
                                teleportController.Base.Position = teleportController.RelativePosition;
                                teleportController.Base.Scale = teleportController.Scale;
                                teleportController.Base.RoomType = teleportController.RoomType;

                                map.Teleports.Add(teleportController.Base);

                                break;
                            }

                        case LockerObject locker:
                            {
                                locker.Base.Position = locker.RelativePosition;
                                locker.Base.Rotation = locker.RelativeRotation;
                                locker.Base.Scale = locker.Scale;
                                locker.Base.RoomType = locker.RoomType;

                                map.Lockers.Add(locker.Base);

                                break;
                            }

                        case SchematicObject schematicObject:
                            {
                                schematicObject.Base.Position = schematicObject.OriginalPosition;
                                schematicObject.Base.Rotation = schematicObject.OriginalRotation;
                                schematicObject.Base.Scale = schematicObject.Scale;
                                schematicObject.Base.RoomType = schematicObject.RoomType;

                                map.Schematics.Add(schematicObject.Base);

                                break;
                            }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            string path = Path.Combine(MapEditorReborn.MapsDir, $"{map.Name}.yml");
            Log.Debug($"Path to file set to: {path}");

            bool prevValue = Config.EnableFileSystemWatcher;
            if (prevValue)
                Config.EnableFileSystemWatcher = false;

            Log.Debug("Trying to serialize the MapSchematic...");
            File.WriteAllText(path, Loader.Serializer.Serialize(map));

            Log.Debug("MapSchematic has been successfully saved to a file!");
            Timing.CallDelayed(1f, () => Config.EnableFileSystemWatcher = prevValue);
        }

        /// <summary>
        /// Gets the <see cref="Serializable.MapSchematic"/> by it's name.
        /// </summary>
        /// <param name="mapName">The name of the map.</param>
        /// <returns><see cref="Serializable.MapSchematic"/> if the file with the map was found, otherwise <see langword="null"/>.</returns>
        public static MapSchematic? GetMapByName(string mapName)
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
        /// Gets the <see cref="SchematicObjectDataList"/> by it's name.
        /// </summary>
        /// <param name="schematicName">The name of the map.</param>
        /// <returns><see cref="SchematicObjectDataList"/> if the file with the schematic was found, otherwise <see langword="null"/>.</returns>
        public static SchematicObjectDataList GetSchematicDataByName(string schematicName)
        {
            string dirPath = Path.Combine(MapEditorReborn.SchematicsDir, schematicName);
            if (!Directory.Exists(dirPath))
                return null;

            string schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
            if (!File.Exists(schematicPath))
                return null;

            SchematicObjectDataList data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicPath));
            data.Path = dirPath;

            return data;
        }

        /// <summary>
        /// Merges two or more <see cref="MapSchematic"/>s into one.
        /// </summary>
        /// <param name="name">The name of the merged map schematic.</param>
        /// <param name="maps">The list of <see cref="MapSchematic"/>s to merge.</param>
        /// <returns>A new <see cref="MapSchematic"/> object that contains all of the elements from the input maps.</returns>
        public static MapSchematic MergeMaps(string name, List<MapSchematic> maps)
        {
            MapSchematic outputMap = new (name);

            foreach (MapSchematic map in maps)
            {
                outputMap.Doors.AddRange(map.Doors);
                outputMap.WorkStations.AddRange(map.WorkStations);
                outputMap.ItemSpawnPoints.AddRange(map.ItemSpawnPoints);
                outputMap.PlayerSpawnPoints.AddRange(map.PlayerSpawnPoints);
                outputMap.RagdollSpawnPoints.AddRange(map.RagdollSpawnPoints);
                outputMap.ShootingTargets.AddRange(map.ShootingTargets);
                outputMap.Primitives.AddRange(map.Primitives);
                outputMap.LightSources.AddRange(map.LightSources);
                outputMap.RoomLights.AddRange(map.RoomLights);
                outputMap.Teleports.AddRange(map.Teleports);
                outputMap.Lockers.AddRange(map.Lockers);
                outputMap.Schematics.AddRange(map.Schematics);
            }

            return outputMap;
        }

        internal static bool TryGetRandomMap(List<string> mapNames, out MapSchematic mapSchematic)
        {
            mapSchematic = null;

            if (mapNames.Count == 0)
                return false;

            if (mapNames[0] == UnloadKeyword)
                return true;

            List<string> mapNamesCopy = ListPool<string>.Shared.Rent(mapNames);
            mapNamesCopy.Remove(UnloadKeyword);

            do
            {
                string chosenMapName = mapNamesCopy[Random.Range(0, mapNamesCopy.Count)];
                MapSchematic chosenMap = GetMapByName(chosenMapName);

                if (chosenMap is not {IsValid: true})
                {
                    mapNamesCopy.Remove(chosenMapName);
                    continue;
                }

                mapSchematic = chosenMap;
                ListPool<string>.Shared.Return(mapNamesCopy);
                return true;
            }
            while (mapNamesCopy.Count > 0);

            ListPool<string>.Shared.Return(mapNamesCopy);
            return mapNames.Contains(UnloadKeyword);
        }

        private const string UnloadKeyword = "UNLOAD";
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}
