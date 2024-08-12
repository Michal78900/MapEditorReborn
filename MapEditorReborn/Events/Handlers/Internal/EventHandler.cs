// -----------------------------------------------------------------------
// <copyright file="EventHandler.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using API.Enums;
    using API.Features;
    using API.Features.Objects;
    using API.Features.Objects.Vanilla;
    using API.Features.Serializable;
    using EventArgs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader;
    using Interactables.Interobjects.DoorUtils;
    using InventorySystem.Items.Firearms.Modules;
    using MEC;
    using UnityEngine;
    using static API.API;
    using Config = Configs.Config;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    internal static class EventHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            SpawnedObjects.Clear();
            AttachedSchemats.Clear();
            PopulateRoomTypeLists();
            GetObjectPrefabs();

            PlayerSpawnPointObject.ResetSpawnpoints();
            VanillaDoorObject.NameUnnamedDoors();

            AutoLoadMaps(Config.LoadMapOnEvent.OnGenerated);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers() => RoomLightObject.RegisterFlickerableLights();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted()"/>
        internal static void OnRoundStarted() => AutoLoadMaps(Config.LoadMapOnEvent.OnRoundStarted);

        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnDecontaminating"/>
        internal static void OnDecontaminating(DecontaminatingEventArgs _)
        {
            UnloadMaps(true);
            AutoLoadMaps(Config.LoadMapOnEvent.OnDecontaminating);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnDetonated()"/>
        internal static void OnWarheadDetonated()
        {
            UnloadMaps(false);
            AutoLoadMaps(Config.LoadMapOnEvent.OnWarheadDetonated);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnShooting(ShootingEventArgs)"/>
        internal static void OnShootingDoor(ShootingEventArgs ev)
        {
            if (ev.Player.CurrentItem is null)
            {
                return;
            }

            Vector3 forward = ev.Player.CameraTransform.forward;
            Vector3 position = ev.Player.CameraTransform.position;
            if (ev.Player.CurrentItem is not Exiled.API.Features.Items.Firearm firearm)
                return;

            float maxDistance = firearm.Base.BaseStats.MaxDistance();
            if (!Physics.Raycast(position, forward, out RaycastHit raycastHit, maxDistance, StandardHitregBase.HitregMask))
                return;

            DoorObject doorObject = raycastHit.collider.GetComponentInParent<DoorObject>();
            if (doorObject is null)
                return;

            if (doorObject.Base.IgnoredDamageSources.HasFlagFast(DoorDamageType.Weapon) || doorObject._remainingHealth <= 0f)
                return;

            doorObject._remainingHealth -= firearm.Base.BaseStats.DamageAtDistance(firearm.Base, raycastHit.distance) * 0.1f;
            if (doorObject._remainingHealth <= 0f)
                doorObject.BreakDoor();

            ev.Player.ShowHitMarker();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (!ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetObject shootingTargetComponent) || shootingTargetComponent == null)
                return;

            if (ev.TargetButton == ShootingTargetButton.Remove)
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
                        Log.Debug("Trying to deserialize the file... (called by FileSytemWatcher)");
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDamagingShootingTarget(DamagingShootingTargetEventArgs)"/>
        internal static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetObject shootingTargetComponent) && shootingTargetComponent.Base.IsFunctional)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        internal static void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!PickupsLocked.Contains(ev.Pickup.Serial))
                return;

            ev.IsAllowed = false;
            Schematic.OnButtonInteract(new ButtonInteractedEventArgs(ev.Pickup, ev.Player, ev.Pickup.Base.GetComponentInParent<SchematicObject>()));
        }

        internal static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            if (!PickupsUsesLeft.TryGetValue(ev.Pickup.Serial, out int usesLeft))
                return;

            if (usesLeft >= 0)
            {
                usesLeft--;
                if (usesLeft <= 0)
                {
                    PickupsUsesLeft.Remove(ev.Pickup.Serial);
                    return;
                }

                PickupsUsesLeft[ev.Pickup.Serial] = usesLeft;
            }

            ev.IsAllowed = false;
            ev.Pickup.IsLocked = false;

            if (CustomItem.TryGet(ev.Pickup, out CustomItem customItem))
                CustomItem.Get(customItem.Id).Give(ev.Player);
            else
                ev.Player.AddItem(Item.Create(ev.Pickup.Type, ev.Player));
        }

        internal static void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (!ev.Locker.TryGetComponent(out LockerObject locker))
                return;

            if (!locker.Base.AllowedRoleTypes.Contains(ev.Player.Role.Type.ToString()))
            {
                ev.IsAllowed = false;
                return;
            }

            if (!locker.Base.InteractLock)
                return;

            if (locker._usedChambers.Contains(ev.Chamber))
            {
                ev.IsAllowed = false;
                return;
            }

            locker._usedChambers.Add(ev.Chamber);
        }

        private static void AutoLoadMaps(List<string> names)
        {
            Timing.CallDelayed(1f, () =>
            {
                try
                {
                    switch (Config.LoadMapOnEventMode)
                    {
                        case LoadMapOnEventMode.Random:
                            if (MapUtils.TryGetRandomMap(names, out var mapSchematic))
                            {
                                CurrentLoadedMap = mapSchematic;
                            }

                            break;

                        case LoadMapOnEventMode.Merge:
                            var maps = ListPool<MapSchematic>.Pool.Get();

                            foreach (var name in names)
                            {
                                var map = MapUtils.GetMapByName(name);

                                if (map is null)
                                {
                                    Log.Warn($"Map named {name} does not exist. Skipping...");
                                    continue;
                                }

                                maps.Add(map);
                            }

                            CurrentLoadedMap = MapUtils.MergeMaps("mer_autoload", maps);

                            ListPool<MapSchematic>.Pool.Return(maps);

                            break;
                    }
                }
                catch (Exception error)
                {
                    Log.Error(error);
                }
            });
        }

        private static void UnloadMaps(bool isDecontaminating)
        {
            var zone = isDecontaminating
                ? ZoneType.LightContainment
                : ZoneType.LightContainment | ZoneType.HeavyContainment | ZoneType.Entrance;

            foreach (var mapEditorObject in SpawnedObjects.ToList())
            {
                // Необходимо обратить первый раз, чтобы currentRoom не был null
                var roomType = mapEditorObject.RoomType;

                if (!zone.HasFlag(mapEditorObject.CurrentRoom.Zone))
                {
                    continue;
                }

                try
                {
                    mapEditorObject.Destroy();
                }
                catch (Exception)
                {
                    // Ignored
                }

                SpawnedObjects.Remove(mapEditorObject);
            }
        }

        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}