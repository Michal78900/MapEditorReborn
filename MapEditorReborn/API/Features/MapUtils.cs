// -----------------------------------------------------------------------
// <copyright file="MapUtils.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using PluginAPI.Core;

namespace MapEditorReborn.API.Features;

using System;
using System.Collections.Generic;
using System.IO;
using Events.Handlers.Internal;
using MEC;
using Objects;
using Serializable;

using static API;

using Config = Configs.Config;

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

        var mapSchematic = GetMapByName(mapName);
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
    public static void LoadMap(MapSchematic map)
    {
        if (map is not null && !map.IsValid)
        {
            Log.Warning($"{map.Name} couldn't be loaded because one of it's object is in RoomType that didn't spawn this round!");
            return;
        }

        API.MapSchematic = map;

        Log.Info("Trying to load the map...");

        foreach (var mapEditorObject in SpawnedObjects)
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

        Log.Info("Destroyed all map's GameObjects and indicators.");

        // This is to bring vanilla spawnpoints to their previous state.
        PlayerSpawnPointObject.VanillaSpawnPointsDisabled = false;

        // This is to remove selected object hint.
        foreach (Player player in Player.GetPlayers())
            ToolGunHandler.SelectObject(player, null);
        

        if (map == null)
        {
            Log.Info("Map is null. Returning...");
            return;
        }

        foreach (var door in map.Doors)
        {
            Log.Info($"Trying to spawn door at {door.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnDoor(door));
        }

        if (map.Doors.Count > 0)
            Log.Info("All doors have been successfully spawned!");

        foreach (var workstation in map.WorkStations)
        {
            Log.Info($"Spawning workstation at {workstation.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnWorkstation(workstation));
        }

        if (map.WorkStations.Count > 0)
            Log.Info("All workstations have been successfully spawned!");

        foreach (var itemSpawnPoint in map.ItemSpawnPoints)
        {
            Log.Info($"Trying to spawn a item spawn point at {itemSpawnPoint.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnItemSpawnPoint(itemSpawnPoint));
        }

        if (map.ItemSpawnPoints.Count > 0)
            Log.Info("All item spawn points have been spawned!");

        foreach (var playerSpawnPoint in map.PlayerSpawnPoints)
        {
            Log.Info($"Trying to spawn a player spawn point at {playerSpawnPoint.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnPlayerSpawnPoint(playerSpawnPoint));
        }

        if (map.PlayerSpawnPoints.Count > 0)
            Log.Info("All player spawn points have been spawned!");

        PlayerSpawnPointObject.VanillaSpawnPointsDisabled = map.RemoveDefaultSpawnPoints;

        foreach (var ragdollSpawnPoint in map.RagdollSpawnPoints)
        {
            Log.Info($"Trying to spawn a ragdoll spawn point at {ragdollSpawnPoint.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnRagdollSpawnPoint(ragdollSpawnPoint));
        }

        if (map.RagdollSpawnPoints.Count > 0)
            Log.Info("All ragdoll spawn points have been spawned!");

        foreach (var shootingTargetObject in map.ShootingTargets)
        {
            Log.Info($"Trying to spawn a shooting target at {shootingTargetObject.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnShootingTarget(shootingTargetObject));
        }

        if (map.ShootingTargets.Count > 0)
            Log.Info("All shooting targets have been spawned!");

        foreach (var primitiveObject in map.Primitives)
        {
            SpawnedObjects.Add(ObjectSpawner.SpawnPrimitive(primitiveObject));
        }

        foreach (var lightControllerObject in map.RoomLights)
        {
            Log.Info($"Trying to spawn a light controller at {lightControllerObject.RoomType}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnRoomLight(lightControllerObject));
        }

        if (map.RoomLights.Count > 0)
            Log.Info("All light controllers have been spawned!");

        foreach (var lightSourceObject in map.LightSources)
        {
            SpawnedObjects.Add(ObjectSpawner.SpawnLightSource(lightSourceObject));
        }

        foreach (var teleportObject in map.Teleports)
        {
            Log.Info($"Trying to spawn a teleporter at {teleportObject.Position}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnTeleport(teleportObject));
        }

        if (map.Teleports.Count > 0)
            Log.Info("All teleporters have been spawned!");

        foreach (var lockerSerializable in map.Lockers)
        {
            Log.Info($"Trying to spawn a locker at {lockerSerializable.Position}...");
            SpawnedObjects.Add(ObjectSpawner.SpawnLocker(lockerSerializable));
        }

        if (map.Lockers.Count > 0)
            Log.Info("All lockers have been spawned!");

        foreach (var schematicObject in map.Schematics)
        {
            Log.Info($"Trying to spawn a schematic named \"{schematicObject.SchematicName}\" at {schematicObject.RoomType}...");
            MapEditorObject schematic = ObjectSpawner.SpawnSchematic(schematicObject);

            if (schematic == null)
            {
                Log.Warning($"The schematic with \"{schematicObject.SchematicName}\" name does not exist or has an invalid name. Skipping...");
                continue;
            }

            SpawnedObjects.Add(schematic);
        }

        if (map.Schematics.Count > 0)
            Log.Info("All schematics have been spawned!");

        Log.Info("All GameObject have been spawned and the MapSchematic has been fully loaded!");
    }

    /// <summary>
    /// Saves the map to a file.
    /// </summary>
    /// <param name="name">The name of the map.</param>
    public static void SaveMap(string name)
    {
        Log.Info("Trying to save the map...");
        var map = GetMapByName(name);

        if (map == null)
        {
            map = new MapSchematic(name);
        }
        else
        {
            map.CleanupAll();
        }

        Log.Info($"Map name set to \"{map.Name}\"");
        foreach (var spawnedObject in SpawnedObjects)
        {
            try
            {
                if (spawnedObject is IndicatorObject)
                    continue;

                Log.Info($"Trying to save GameObject at {spawnedObject.transform.position}...");

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
                continue;
            }
        }

        var path = Path.Combine(MapEditorReborn.MapsDir, $"{map.Name}.yml");
        Log.Info($"Path to file set to: {path}");

        var prevValue = Config.EnableFileSystemWatcher;
        if (prevValue)
            Config.EnableFileSystemWatcher = false;

        Log.Info("Trying to serialize the MapSchematic...");
        //File.WriteAllText(path, MapEditorReborn.Serializer.Serialize(map));

        Log.Info("MapSchematic has been successfully saved to a file!");
        Timing.CallDelayed(1f, () => Config.EnableFileSystemWatcher = prevValue);
    }

    /// <summary>
    /// Gets the <see cref="Serializable.MapSchematic"/> by it's name.
    /// </summary>
    /// <param name="mapName">The name of the map.</param>
    /// <returns><see cref="Serializable.MapSchematic"/> if the file with the map was found, otherwise <see langword="null"/>.</returns>
    public static MapSchematic GetMapByName(string mapName)
    {
        if (mapName == CurrentLoadedMap?.Name)
            return CurrentLoadedMap;

        var path = Path.Combine(MapEditorReborn.MapsDir, $"{mapName}.yml");

        if (!File.Exists(path))
            return null;

        return null;
        // MapSchematic map = MapEditorReborn.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));
        // map.Name = mapName;
        //
        // return map;
    }

    /// <summary>
    /// Gets the <see cref="SchematicObjectDataList"/> by it's name.
    /// </summary>
    /// <param name="schematicName">The name of the map.</param>
    /// <returns><see cref="SchematicObjectDataList"/> if the file with the schematic was found, otherwise <see langword="null"/>.</returns>
    public static SchematicObjectDataList GetSchematicDataByName(string schematicName)
    {
        var dirPath = Path.Combine(MapEditorReborn.SchematicsDir, schematicName);
        if (!Directory.Exists(dirPath))
            return null;

        var schematicPath = Path.Combine(dirPath, $"{schematicName}.json");
        if (!File.Exists(schematicPath))
            return null;

        var data = Utf8Json.JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicPath));
        data.Path = dirPath;

        return data;
    }

    internal static bool TryGetRandomMap(List<string> mapNames, out MapSchematic mapSchematic)
    {
        mapSchematic = null;

        if (mapNames.Count == 0)
            return false;

        if (mapNames[0] == UnloadKeyword)
            return true;

        var mapNamesCopy = NorthwoodLib.Pools.ListPool<string>.Shared.Rent(mapNames);
        mapNamesCopy.Remove(UnloadKeyword);

        do
        {
            var chosenMapName = mapNamesCopy[UnityEngine.Random.Range(0, mapNamesCopy.Count)];
            var chosenMap = GetMapByName(chosenMapName);

            if (chosenMap is not {IsValid: true})
            {
                mapNamesCopy.Remove(chosenMapName);
                continue;
            }

            mapSchematic = chosenMap;
            NorthwoodLib.Pools.ListPool<string>.Shared.Return(mapNamesCopy);
            return true;
        }
        while (mapNamesCopy.Count > 0);

        NorthwoodLib.Pools.ListPool<string>.Shared.Return(mapNamesCopy);
        return mapNames.Contains(UnloadKeyword);
    }

    private const string UnloadKeyword = "UNLOAD";
    private static readonly Config Config = MapEditorReborn.Singleton.Config;
}