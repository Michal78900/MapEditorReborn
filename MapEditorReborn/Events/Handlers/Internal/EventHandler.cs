namespace MapEditorReborn.Events.Handlers.Internal
{
    using System;
    using System.IO;
    using System.Linq;
    using API.Enums;
    using API.Extensions;
    using API.Features;
    using API.Features.Components;
    using API.Features.Components.ObjectComponents;
    using API.Features.Objects;
    using EventArgs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using Exiled.Events.EventArgs;
    using Exiled.Loader;
    using MapGeneration;
    using MEC;
    using Mirror.LiteNetLib4Mirror;
    using UnityEngine;

    using static API.API;

    using Config = Config;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles mostly EXILED events.
    /// </summary>
    internal static class EventHandler
    {
        /// <inheritdoc cref="Exiled.Events.Handlers.Map.OnGenerated"/>
        internal static void OnGenerated()
        {
            _roomTypes = null;

            SpawnedObjects.Clear();
            ObjectPrefabs.Clear();

            DoorSpawnpoint[] doorList = Object.FindObjectsOfType<DoorSpawnpoint>();
            ObjectPrefabs.Add(ObjectType.LczDoor, doorList.First(x => x.TargetPrefab.name.Contains("LCZ")).TargetPrefab.gameObject);
            ObjectPrefabs.Add(ObjectType.HczDoor, doorList.First(x => x.TargetPrefab.name.Contains("HCZ")).TargetPrefab.gameObject);
            ObjectPrefabs.Add(ObjectType.EzDoor, doorList.First(x => x.TargetPrefab.name.Contains("EZ")).TargetPrefab.gameObject);

            ObjectPrefabs.Add(ObjectType.WorkStation, LiteNetLib4MirrorNetworkManager.singleton.spawnPrefabs.Find(x => x.name == "Work Station"));

            ObjectPrefabs.Add(ObjectType.ItemSpawnPoint, new GameObject("ItemSpawnPointObject"));
            ObjectPrefabs.Add(ObjectType.PlayerSpawnPoint, new GameObject("PlayerSpawnPointObject"));
            ObjectPrefabs.Add(ObjectType.RagdollSpawnPoint, new GameObject("RagdollSpawnPointObject"));
            ObjectPrefabs.Add(ObjectType.DummySpawnPoint, new GameObject("DummySpawnPointObject"));

            ObjectPrefabs.Add(ObjectType.SportShootingTarget, ToysHelper.SportShootingTargetObject.gameObject);
            ObjectPrefabs.Add(ObjectType.DboyShootingTarget, ToysHelper.DboyShootingTargetObject.gameObject);
            ObjectPrefabs.Add(ObjectType.BinaryShootingTarget, ToysHelper.BinaryShootingTargetObject.gameObject);

            ObjectPrefabs.Add(ObjectType.Primitive, ToysHelper.PrimitiveBaseObject.gameObject);
            ObjectPrefabs.Add(ObjectType.LightSource, ToysHelper.LightBaseObject.gameObject);

            ObjectPrefabs.Add(ObjectType.RoomLight, new GameObject("LightControllerObject"));
            ObjectPrefabs.Add(ObjectType.Teleporter, new GameObject("TeleportControllerObject"));

            PlayerSpawnPointComponent.RegisterSpawnPoints();

            Timing.CallDelayed(1f, () =>
            {
                if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnGenerated, out MapSchematic mapSchematic))
                    CurrentLoadedMap = mapSchematic;
            });
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers()"/>
        internal static void OnWaitingForPlayers()
        {
            RoomLightComponent.RegisterFlickerableLights();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted()"/>
        internal static void OnRoundStarted()
        {
            if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnRoundStarted, out MapSchematic mapSchematic))
                CurrentLoadedMap = mapSchematic;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Warhead.OnDetonated()"/>
        internal static void OnWarheadDetonated()
        {
            if (MapUtils.TryGetRandomMap(Config.LoadMapOnEvent.OnWarheadDetonated, out MapSchematic mapSchematic))
                CurrentLoadedMap = mapSchematic;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDroppingItem(DroppingItemEventArgs)"/>
        internal static void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (ev.Item.IsToolGun() && ev.IsThrown)
            {
                ev.IsAllowed = false;

                ToolGuns[ev.Player.CurrentItem.Serial]++;

                if ((int)ToolGuns[ev.Player.CurrentItem.Serial] > 14)
                {
                    ToolGuns[ev.Player.CurrentItem.Serial] = 0;
                }

                ObjectType mode = ToolGuns[ev.Player.CurrentItem.Serial];

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
                    ObjectType mode = ToolGuns[ev.Shooter.CurrentItem.Serial];

                    if (mode == ObjectType.RoomLight)
                    {
                        Room colliderRoom = Map.FindParentRoom(hit.collider.gameObject);
                        if (SpawnedObjects.FirstOrDefault(x => x is RoomLightComponent light && light.ForcedRoomType == colliderRoom.Type) != null)
                        {
                            ev.Shooter.ShowHint("There can be only one Light Controller per one room type!");
                            return;
                        }
                    }

                    if (ev.Shooter.TryGetSessionVariable(CopiedObjectSessionVarName, out MapEditorObject prefab) && prefab != null)
                    {
                        SpawnedObjects.Add(ObjectSpawner.SpawnPropertyObject(hit.point, prefab));

                        if (MapEditorReborn.Singleton.Config.ShowIndicatorOnSpawn)
                            SpawnedObjects.Last().UpdateIndicator();
                    }
                    else
                    {
                        ToolGunHandler.SpawnObject(hit.point, mode);
                    }
                }

                return;
            }

            if (ToolGunHandler.TryGetMapObject(ev.Shooter, out MapEditorObject mapObject))
            {
                // Deleting the object
                if (!ev.Shooter.HasFlashlightModuleEnabled && !ev.Shooter.IsAimingDownWeapon)
                {
                    ToolGunHandler.DeleteObject(ev.Shooter, mapObject);
                    return;
                }
            }

            // Copying to the ToolGun
            if (!ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                ToolGunHandler.CopyObject(ev.Shooter, mapObject);
                return;
            }

            // Selecting the object
            if (ev.Shooter.HasFlashlightModuleEnabled && ev.Shooter.IsAimingDownWeapon)
            {
                ToolGunHandler.SelectObject(ev.Shooter, mapObject);
                return;
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnInteractingShootingTarget(InteractingShootingTargetEventArgs)"/>
        internal static void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
        {
            if (!ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetComponent shootingTargetComponent) || shootingTargetComponent == null)
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
                        Log.Debug("Trying to deserialize the file... (called by FileSytemWatcher)", Config.Debug);
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

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnAimingDownSight(AimingDownSightEventArgs)"/>
        internal static void OnAimingDownSight(AimingDownSightEventArgs ev)
        {
            if (!ev.Player.CurrentItem.IsToolGun() || (ev.Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null))
                return;

            ev.Player.ShowHint(ToolGunHandler.GetToolGunModeText(ev.Player, ev.AdsIn, ev.Player.HasFlashlightModuleEnabled), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnDamagingShootingTarget(DamagingShootingTargetEventArgs)"/>
        internal static void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev)
        {
            if (ev.ShootingTarget.Base.TryGetComponent(out ShootingTargetComponent shootingTargetComponent) && shootingTargetComponent.Base.IsFunctional)
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs)"/>
        internal static void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
        {
            if (ev.Player == null ||
                (ev.Firearm.FlashlightEnabled && ev.NewState) ||
                (!ev.Firearm.FlashlightEnabled && !ev.NewState) ||
                !ev.Player.CurrentItem.IsToolGun() ||
                (ev.Player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) &&
                mapObject != null))
                return;

            ev.Player.ShowHint(ToolGunHandler.GetToolGunModeText(ev.Player, ev.Player.IsAimingDownWeapon, ev.NewState), 1f);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnUnloadingWeapon(UnloadingWeaponEventArgs)"/>
        internal static void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
        {
            if (!ev.Firearm.IsToolGun())
                return;

            ev.IsAllowed = false;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        internal static void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (!ItemSpawnPointComponent.LockedPickups.Contains(ev.Pickup))
                return;

            ev.IsAllowed = false;
            Schematic.OnButtonInteract(new ButtonInteractedEventArgs(ev.Pickup, ev.Player, ev.Pickup.Base.GetComponentInParent<SchematicObjectComponent>()));
        }

        private static readonly Config Config = MapEditorReborn.Singleton.Config;
        private static readonly Translation Translation = MapEditorReborn.Singleton.Translation;
    }
}