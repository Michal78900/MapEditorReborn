// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using AdminToys;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.Exiled.Features;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Exiled.Features.Pickups;
using MapEditorReborn.Exiled.Features.Toys;
using MapEditorReborn.Factories;
using MapGeneration.Distributors;
using NorthwoodLib.Pools;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Interfaces;
using PluginAPI.Enums;

namespace MapEditorReborn.Events.Handlers.Internal;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using API.Enums;
using API.Extensions;
using API.Features;
using API.Features.Objects;
using API.Features.Serializable;
using Configs;
using EventArgs;
using MapGeneration;
using MEC;
using Mirror;
using UnityEngine;
using static API.API;
using Config = Configs.Config;
using Object = UnityEngine.Object;

/// <summary>
/// Handles mostly EXILED events.
/// </summary>
internal class EventHandler
{
    [PluginEvent(ServerEventType.MapGenerated)]
    internal void OnGenerated()
    {
        RoomTypes = null;
        SpawnedObjects.Clear();

        GenerateRooms();
        Log.Info($"Loaded {Room.List.Count()} rooms.");

        Dictionary<ObjectType, GameObject> objectList = new(21);

        var doorList = Object.FindObjectsOfType<DoorSpawnpoint>();

        objectList.Add(ObjectType.LczDoor, doorList.First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject);
        objectList.Add(ObjectType.HczDoor, doorList.First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject);
        objectList.Add(ObjectType.EzDoor, doorList.First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject);
        
        objectList.Add(ObjectType.WorkStation, NetworkClient.prefabs.Values.First(x => x.name == "Spawnable Work Station Structure"));

        objectList.Add(ObjectType.ItemSpawnPoint, new GameObject("ItemSpawnPointObject"));
        objectList.Add(ObjectType.PlayerSpawnPoint, new GameObject("PlayerSpawnPointObject"));
        objectList.Add(ObjectType.RagdollSpawnPoint, new GameObject("RagdollSpawnPointObject"));
        objectList.Add(ObjectType.DummySpawnPoint, new GameObject("DummySpawnPointObject"));

        objectList.Add(ObjectType.SportShootingTarget, ToysHelper.SportShootingTargetObject.gameObject);
        objectList.Add(ObjectType.DboyShootingTarget, ToysHelper.DboyShootingTargetObject.gameObject);
        objectList.Add(ObjectType.BinaryShootingTarget, ToysHelper.BinaryShootingTargetObject.gameObject);

        objectList.Add(ObjectType.Primitive, ToysHelper.PrimitiveBaseObject.gameObject);
        objectList.Add(ObjectType.LightSource, ToysHelper.LightBaseObject.gameObject);

        objectList.Add(ObjectType.RoomLight, new GameObject("LightControllerObject"));

        var teleportPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        teleportPrefab.name = "TeleportObject";
        objectList.Add(ObjectType.Teleporter, teleportPrefab);

        objectList.Add(ObjectType.PedestalLocker, NetworkClient.prefabs.Values.First(x => x.name == "Scp500PedestalStructure Variant"));
        objectList.Add(ObjectType.LargeGunLocker, NetworkClient.prefabs.Values.First(x => x.name == "LargeGunLockerStructure"));
        objectList.Add(ObjectType.RifleRackLocker, NetworkClient.prefabs.Values.First(x => x.name == "RifleRackStructure"));
        objectList.Add(ObjectType.MiscLocker, NetworkClient.prefabs.Values.First(x => x.name == "MiscLocker"));
        objectList.Add(ObjectType.MedkitLocker, NetworkClient.prefabs.Values.First(x => x.name == "RegularMedkitStructure"));
        objectList.Add(ObjectType.AdrenalineLocker, NetworkClient.prefabs.Values.First(x => x.name == "AdrenalineMedkitStructure"));
        
        ObjectPrefabs = new ReadOnlyDictionary<ObjectType, GameObject>(objectList);
        Log.Info(ObjectPrefabs.Keys.ToList().Count().ToString());
        
        PlayerSpawnPointObject.RegisterSpawnPoints();

        Timing.CallDelayed(1f, () =>
        {
            try
            {
                if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnGenerated, out var mapSchematic))
                    CurrentLoadedMap = mapSchematic;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        });
    }
    
    private static void GenerateRooms()
    {
        // Get bulk of rooms with sorted.
        List<RoomIdentifier> roomIdentifiers = ListPool<RoomIdentifier>.Shared.Rent(RoomIdentifier.AllRoomIdentifiers);

        // If no rooms were found, it means a plugin is trying to access this before the map is created.
        if (roomIdentifiers.Count == 0)
            throw new InvalidOperationException("Plugin is trying to access Rooms before they are created.");

        foreach (var roomIdentifier in roomIdentifiers)
            Room.RoomIdentifierToRoom.Add(roomIdentifier, Room.CreateComponent(roomIdentifier.gameObject));

        ListPool<RoomIdentifier>.Shared.Return(roomIdentifiers);
    }

    [PluginEvent(ServerEventType.WaitingForPlayers)]
    internal void OnWaitingForPlayers()
    {
        RoomLightObject.RegisterFlickerableLights();
    }

    [PluginEvent(ServerEventType.RoundStart)]
    internal void OnRoundStarted()
    {
        if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnRoundStarted, out var mapSchematic))
            CurrentLoadedMap = mapSchematic;
    }

    [PluginEvent(ServerEventType.WarheadDetonation)]
    internal void OnWarheadDetonated()
    {
        if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnWarheadDetonated, out var mapSchematic))
            CurrentLoadedMap = mapSchematic;
    }

    [PluginEvent(ServerEventType.PlayerDropItem)]
    internal void OnDroppingItem(MERPlayer player, ItemBase item)
    {
        // if (item.IsToolGun() && ev.IsThrown)
        // {
        //     ev.IsAllowed = false;
        //
        //     ToolGuns[Player.CurrentItem.Serial]++;
        //
        //     if ((int)ToolGuns[Player.CurrentItem.Serial] > ObjectPrefabs.Count - 1)
        //     {
        //         ToolGuns[Player.CurrentItem.Serial] = 0;
        //     }
        //
        //     var mode = ToolGuns[Player.CurrentItem.Serial];
        //
        //     // ev.Player.ShowHint(!ev.Player.IsAimingDownWeapon && ev.Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>", 1f);
        //     Player.ClearBroadcasts();
        //     Player.Broadcast(1, !Player.IsAimingDownWeapon && Player.HasFlashlightModuleEnabled ? $"{Translation.ModeCreating}\n<b>({mode})</b>" : $"<b>{mode}</b>");
        // }
    }

    [PluginEvent(ServerEventType.PlayerShotWeapon)]
    internal void OnShooting(IPlayer player, Firearm firearm)
    {
        // if (!ev.Shooter.CurrentItem.IsToolGun())
        //     return;
        //
        // ev.IsAllowed = false;
        //
        // // Creating an object
        // if (ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
        // {
        //     Vector3 forward = ev.Shooter.CameraTransform.forward;
        //     if (Physics.Raycast(ev.Shooter.CameraTransform.position + forward, forward, out var hit, 100f))
        //     {
        //         var mode = ToolGuns[ev.Shooter.CurrentItem.Serial];
        //
        //         if (mode == ObjectType.RoomLight)
        //         {
        //             Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
        //             if (SpawnedObjects.FirstOrDefault(x => x is RoomLightObject light && light.ForcedRoomType == colliderRoom.Type) != null)
        //             {
        //                 ev.Shooter.ShowHint("There can be only one Light Controller per one room type!");
        //                 return;
        //             }
        //         }
        //
        //         if (ev.Shooter.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
        //         {
        //             SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));
        //
        //             if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
        //                 SpawnedObjects.Last().UpdateIndicator();
        //         }
        //         else
        //         {
        //             ToolGunHandler.SpawnObject(hit.point, mode);
        //         }
        //     }
        //
        //     return;
        // }
        //
        // if (ToolGunHandler.TryGetMapObject(ev.Shooter, out var mapObject))
        // {
        //     // Deleting the object
        //     if (!ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
        //     {
        //         ToolGunHandler.DeleteObject(ev.Shooter, mapObject);
        //         return;
        //     }
        // }
        //
        // // Copying to the ToolGun
        // if (!ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
        // {
        //     ToolGunHandler.CopyObject(ev.Shooter, mapObject);
        //     return;
        // }
        //
        // // Selecting the object
        // if (ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
        // {
        //     ToolGunHandler.SelectObject(ev.Shooter, mapObject);
        //     return;
        // }
    }

    [PluginEvent(ServerEventType.PlayerInteractShootingTarget)]
    internal void OnInteractingShootingTarget(MERPlayer player, ShootingTarget shootingTarget)
    {
        // if (!ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetObject shootingTargetComponent) || shootingTargetComponent == null)
        //     return;
        //
        // if (ev.TargetButton == ShootingTargetButton.Remove)
        //     ev.IsAllowed = false;
    }

    /// <inheritdoc cref="FileSystemWatcher.OnChanged(FileSystemEventArgs)"/>
    // internal static void OnFileChanged(object sender, FileSystemEventArgs ev)
    // {
    //     if (!Config.EnableFileSystemWatcher)
    //         return;
    //
    //     var fileName = Path.GetFileNameWithoutExtension(ev.Name);
    //
    //     if (fileName == CurrentLoadedMap?.Name)
    //     {
    //         Timing.CallDelayed(0.1f, () =>
    //         {
    //             try
    //             {
    //                 Log.Info("Trying to deserialize the file... (called by FileSytemWatcher)");
    //                 CurrentLoadedMap = MapEditorReborn.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(ev.FullPath));
    //                 CurrentLoadedMap.Name = fileName;
    //             }
    //             catch (Exception e)
    //             {
    //                 Log.Error($"You did something wrong in your MapSchematic file!\n{e.Message}");
    //             }
    //         });
    //     }
    // }

    [PluginEvent(ServerEventType.PlayerAimWeapon)]
    internal void OnAimingDownSight(IPlayer player, Firearm firearm, bool isAiming)
    {
        // if (!Player.CurrentItem.IsToolGun() || (Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
        //     return;
        //
        // Player.ShowHint(ToolGunHandler.GetToolGunModeText(Player, ev.AdsIn, Player.HasFlashlightModuleEnabled), 1f);
    }

    [PluginEvent(ServerEventType.PlayerDamagedShootingTarget)]
    internal bool OnDamagingShootingTarget(MERPlayer player, ShootingTarget shootingTarget, DamageHandlerBase damageHandlerBase, float damageAmount)
    => !shootingTarget.TryGetComponent(out ShootingTargetObject shootingTargetComponent) || !shootingTargetComponent.Base.IsFunctional;

    // /// <inheritdoc cref="Player.OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs)"/>
    // internal static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
    // {
    //     if (Player == null ||
    //         (ev.Firearm.FlashlightEnabled && ev.NewState) ||
    //         (!ev.Firearm.FlashlightEnabled && !ev.NewState) ||
    //         !Player.CurrentItem.IsToolGun() ||
    //         (Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) &&
    //          mapObject != null))
    //         return;
    //
    //     Player.ShowHint(ToolGunHandler.GetToolGunModeText(Player, Player.IsAimingDownWeapon, ev.NewState), 1f);
    // }

    [PluginEvent(ServerEventType.PlayerUnloadWeapon)]
    internal void OnUnloadingWeapon(IPlayer player, Firearm firearm)
    {
        // if (!ev.Firearm.IsToolGun())
        //     return;
        //
        // ev.IsAllowed = false;
    }

    [PluginEvent(ServerEventType.PlayerSearchPickup)]
    internal bool OnSearchingPickup(MERPlayer player, ItemPickupBase itemPickupBase)
    {
        if (!PickupsLocked.Contains(itemPickupBase.Info.Serial))
            return false;

        Pickup.BaseToPickup.TryGetValue(itemPickupBase, out var pickup);
        
        Schematic.OnButtonInteract(new ButtonInteractedEventArgs(pickup, player, itemPickupBase.GetComponentInParent<SchematicObject>()));
        return true;
    }

    [PluginEvent(ServerEventType.PlayerSearchedPickup)]
    internal bool OnPickingUpItem(MERPlayer player, ItemPickupBase itemPickupBase)
    {
        if (!PickupsUsesLeft.TryGetValue(itemPickupBase.Info.Serial, out var usesLeft))
            return false;

        if (usesLeft >= 0)
        {
            usesLeft--;
            if (usesLeft <= 0)
            {
                PickupsUsesLeft.Remove(itemPickupBase.Info.Serial);
                return false;
            }

            PickupsUsesLeft[itemPickupBase.Info.Serial] = usesLeft;
        }
        
        itemPickupBase.Info.Locked = false;
        
        player.AddItem(itemPickupBase.Info.ItemId);

        return true;
    }

    [PluginEvent(ServerEventType.PlayerInteractLocker)]
    internal bool OnInteractingLocker(MERPlayer player, Locker locker, LockerChamber lockerChamber, bool canOpen)
    {
        if (!locker.TryGetComponent(out LockerObject lockerObj))
            return true;

        if (!lockerObj.Base.AllowedRoleTypes.Contains(player.Role.ToString()))
            return true;
        
        if (!lockerObj.Base.InteractLock)
            return true;

        if (lockerObj._usedChambers.Contains(lockerChamber))
            return true;

        lockerObj._usedChambers.Add(lockerChamber);
        return false;
    }

    private static readonly Config Config = MapEditorReborn.Singleton.Config;
    private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
}